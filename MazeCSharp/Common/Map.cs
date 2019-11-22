using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Common
{
    public class Map
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public Bitmap Image { get; set; }

        public Color[][] ImageColors { get; set; }

        public Color[][] PreviewImageColors { get; set; }

        public ConcurrentDictionary<string, MapNode> Nodes { get; set; }

        public ConcurrentStack<Point> PreviewPixelBuffer = new ConcurrentStack<Point>();
        #endregion Properties..

        #region Constructors..
        public Map(Bitmap mazeImage)
        {
            Image = mazeImage;
            Nodes = new ConcurrentDictionary<string, MapNode>();
        }
        #endregion Constructors..

        #region Methods..
        public Image DrawPreview()
        {
            // Empty the buffer
            while (PreviewPixelBuffer.TryPop(out Point point))
            {
                PreviewImageColors[point.X][point.Y] = Color.Red;
            }

            int RgbIndex = 0;
            byte[] RgbData = new byte[PreviewImageColors.Length * PreviewImageColors[0].Length * 4];
            for (int i = 0; i < PreviewImageColors.Length; i++)
            {
                for (int j = 0; j < PreviewImageColors[i].Length; j++)
                {
                    RgbData[RgbIndex++] = PreviewImageColors[i][j].B;
                    RgbData[RgbIndex++] = PreviewImageColors[i][j].G;
                    RgbData[RgbIndex++] = PreviewImageColors[i][j].R;
                    RgbData[RgbIndex++] = PreviewImageColors[i][j].A;
                }
            }

            Bitmap ImageCopy = new Bitmap(Image);
            var ImageBitmapData = ImageCopy.LockBits(new Rectangle(0, 0, ImageCopy.Width, ImageCopy.Height), ImageLockMode.ReadWrite, ImageCopy.PixelFormat);
            Marshal.Copy(RgbData, 0, ImageBitmapData.Scan0, RgbData.Length);
            ImageCopy.UnlockBits(ImageBitmapData);
            return ImageCopy;
        }

        public void DrawSolution(bool floodFill)
        {
            if (!floodFill)
            {
                MapNode EndNode = Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
                foreach (var segment in EndNode.GetPathSegments())
                {
                    string[] Positions = segment?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string position in Positions)
                    {
                        int X = Convert.ToInt32(position.Split(',')[0]);
                        int Y = Convert.ToInt32(position.Split(',')[1]);
                        ImageColors[Y][X] = Color.Red;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, MapNode> node in Nodes)
                {
                    if (node.Value.NodeValue == 2)
                    {
                        string[] Position = node.Key.Split(',');
                        int X = Convert.ToInt32(Position[0]);
                        int Y = Convert.ToInt32(Position[1]);
                        ImageColors[X][Y] = Color.Red;
                    }
                }
            }
        }

        public void ExportSolution(string fileName, string selectedAlgorithm, double elapsedMilliseconds)
        {
            string SavePath = Path.Combine(Environment.CurrentDirectory, "Solutions", $"{fileName}_{selectedAlgorithm}_{elapsedMilliseconds / 1000}.png");

            int RgbIndex = 0;
            byte[] RgbData = new byte[ImageColors.Length * ImageColors[0].Length * 4];
            for (int i = 0; i < ImageColors.Length; i++)
            {
                for (int j = 0; j < ImageColors[i].Length; j++)
                {
                    RgbData[RgbIndex++] = ImageColors[i][j].B;
                    RgbData[RgbIndex++] = ImageColors[i][j].G;
                    RgbData[RgbIndex++] = ImageColors[i][j].R;
                    RgbData[RgbIndex++] = ImageColors[i][j].A;
                }
            }

            Bitmap ImageBitmap = new Bitmap(ImageColors[0].Length, ImageColors.Length, PixelFormat.Format32bppArgb);
            var ImageBitmapData = ImageBitmap.LockBits(new Rectangle(0, 0, ImageBitmap.Width, ImageBitmap.Height), ImageLockMode.ReadWrite, ImageBitmap.PixelFormat);
            Marshal.Copy(RgbData, 0, ImageBitmapData.Scan0, RgbData.Length);
            ImageBitmap.UnlockBits(ImageBitmapData);
            ImageBitmap.Save(SavePath);
        }

        public void InitializeImageColors()
        {
            ImageColors = new Color[Image.Width][];
            PreviewImageColors = new Color[Image.Width][];

            for (int i = 0; i < Image.Width; i++)
            {
                ImageColors[i] = new Color[Image.Height];
                PreviewImageColors[i] = new Color[Image.Height];

                for (int j = 0; j < Image.Height; j++)
                {
                    ImageColors[i][j] = Image.GetPixel(j, i);
                    PreviewImageColors[i][j] = Image.GetPixel(j, i);
                }
            }
        }

        public void InitializeNodes()
        {
            // Populate Map
            Parallel.For(0, ImageColors.Length, (i) =>
            {
                for (int j = 0; j < ImageColors[i].Length; j++)
                {
                    string Position = $"{i},{j}";

                    // 0 = White, -1 = Black
                    int ColorValue = ImageColors[i][j] == Color.FromArgb(0, 0, 0) ? -1 : 0;
                    bool IsStartNode = i == 0 && ColorValue == 0;
                    bool IsEndNode = i == ImageColors.Length - 1 && ColorValue == 0;

                    Nodes[Position] = new MapNode()
                    {
                        Position = new Point() { X = j, Y = i },
                        NodeValue = ColorValue,
                        IsStartNode = IsStartNode,
                        IsEndNode = IsEndNode
                    };
                }
            });

            // Process Map and find all nodes
            Parallel.For(0, ImageColors.Length, (i) =>
            {
                for (int j = 0; j < ImageColors[i].Length; j++)
                {
                    string Position = $"{i},{j}";
                    string NeighborPosition = string.Empty;

                    // North
                    if (i != 0)
                    {
                        NeighborPosition = $"{i - 1},{j}";
                        if (Nodes[NeighborPosition].NodeValue == 0)
                        {
                            Nodes[Position].NorthNode = Nodes[NeighborPosition];
                        }
                    }

                    // West
                    if (j != 0)
                    {
                        NeighborPosition = $"{i},{j - 1}";
                        if (Nodes[NeighborPosition].NodeValue == 0)
                        {
                            Nodes[Position].WestNode = Nodes[NeighborPosition];
                        }
                    }

                    // South
                    if (i != ImageColors.Length - 1)
                    {
                        NeighborPosition = $"{i + 1},{j}";
                        if (Nodes[NeighborPosition].NodeValue == 0)
                        {
                            Nodes[Position].SouthNode = Nodes[NeighborPosition];
                        }
                    }

                    // East
                    if (j != ImageColors[i].Length - 1)
                    {
                        NeighborPosition = $"{i},{j + 1}";
                        if (Nodes[NeighborPosition].NodeValue == 0)
                        {
                            Nodes[Position].EastNode = Nodes[NeighborPosition];
                        }
                    }
                }
            });
        }

        public void RefreshNodeCollection()
        {
            MapNode StartNode = Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value;
            MapNode EndNode = Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            Nodes.Clear();
            Nodes[EndNode.Position.ToString()] = EndNode;

            foreach (var segment in EndNode.GetPathSegments())
            {
                string[] Positions = segment?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var position in Positions)
                {
                    Nodes[position] = new MapNode()
                    {
                        Position = Point.FromString(position),
                        NodeValue = EndNode.NodeValue,
                        IsStartNode = StartNode.Position.ToString() == position,
                        IsEndNode = EndNode.Position.ToString() == position
                    };
                }
            }
        }
        #endregion Methods..
    }
}
