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

        // [Row][Column]
        public ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>> Nodes { get; set; }

        public ConcurrentStack<Point> PreviewPixelBuffer = new ConcurrentStack<Point>();

        public MapNode StartNode { get; set; }

        public MapNode EndNode { get; set; }
        #endregion Properties..

        #region Constructors..
        public Map()
        {
            Nodes = new ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>>();
        }

        public Map(Bitmap mazeImage)
        {
            Image = mazeImage;
            Nodes = new ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>>();
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
                foreach (var segment in EndNode.GetPathSegments())
                {
                    string[] Positions = segment?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string position in Positions)
                    {
                        int X = Convert.ToInt32(position.Split(',')[0]);
                        int Y = Convert.ToInt32(position.Split(',')[1]);
                        ImageColors[X][Y] = Color.Red;
                    }
                }
            }
            else
            {
                foreach (var nodeRow in Nodes)
                {
                    foreach (MapNode node in nodeRow.Value.Values)
                    {
                        if (node.NodeValue == 2)
                        {
                            ImageColors[node.Position.X][node.Position.Y] = Color.Red;
                        }
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
            // Populate map
            //Parallel.For(0, ImageColors.Length, (i) =>
            for (int i = 0; i < ImageColors.Length; i++)
            {
                for (int j = 0; j < ImageColors[i].Length; j++)
                {
                    string Position = $"{i},{j}";

                    // 0 = White, -1 = Black
                    int ColorValue = ImageColors[i][j] == Color.FromArgb(0, 0, 0) ? -1 : 0;
                    if (ColorValue != -1)
                    {
                        bool IsStartNode = i == 0;
                        bool IsEndNode = i == ImageColors.Length - 1;

                        if (!Nodes.ContainsKey((int)i))
                        {
                            Nodes[i] = new ConcurrentDictionary<int, MapNode>();
                        }

                        MapNode Node = new MapNode()
                        {
                            Position = new Point(i, j),
                            NodeValue = 0,
                        };

                        Nodes[i][j] = Node;

                        StartNode = IsStartNode ? Node : StartNode;
                        EndNode = IsEndNode ? Node : EndNode;
                    }
                }
            }

            // Build node relationships
            //Parallel.For(0, ImageColors.Length, (i) =>
            for (int i = 0; i < ImageColors.Length; i++)
            {
                for (int j = 0; j < ImageColors[i].Length; j++)
                {
                    Point NeighborPosition = null;
                    if (Nodes.ContainsKey(i) && Nodes[i].ContainsKey(j))
                    {
                        // North
                        if (i != 0)
                        {
                            NeighborPosition = new Point(i - 1, j);
                            if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && Nodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                Nodes[i][j].NorthNode = NeighborPosition;
                            }
                        }

                        // East
                        if (j != ImageColors[i].Length - 1)
                        {
                            NeighborPosition = new Point(i, j + 1);
                            if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && Nodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                Nodes[i][j].EastNode = NeighborPosition;
                            }
                        }

                        // South
                        if (i != ImageColors.Length - 1)
                        {
                            NeighborPosition = new Point(i + 1, j);
                            if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && Nodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                Nodes[i][j].SouthNode = NeighborPosition;
                            }
                        }

                        // West
                        if (j != 0)
                        {
                            NeighborPosition = new Point(i, j - 1);
                            if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && Nodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                Nodes[i][j].WestNode = NeighborPosition;
                            }
                        }
                    }
                }
            }
        }
        #endregion Methods..
    }
}
