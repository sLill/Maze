using ApplicationManager;
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
        private List<string> _SolutionPath = null;
        #endregion Member Variables..

        #region Properties..
        public Bitmap Image { get; set; }

        public Color[][] ImageColors { get; set; }

        public Color[][] PreviewImageColors { get; set; }

        // [Row][Column]
        public ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>> Nodes { get; set; }

        public ConcurrentStack<Point> PreviewPixelBuffer = new ConcurrentStack<Point>();

        public Point StartNodePosition { get; private set; }

        public Point EndNodePosition { get; private set; }
        #endregion Properties..

        #region Constructors..
        public Map(List<string> solutionPath)
        {
            Nodes = new ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>>();
            _SolutionPath = solutionPath;
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
                foreach (var segment in Nodes[EndNodePosition.X][EndNodePosition.Y].GetPathSegments())
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

        private List<Point> InitializeImageColors()
        {
            List<Point> OpenNodes = new List<Point>();

            ImageColors = new Color[Image.Width][];
            PreviewImageColors = new Color[Image.Width][];

            for (int i = 0; i < Image.Width; i++)
            {
                ImageColors[i] = new Color[Image.Height];
                PreviewImageColors[i] = new Color[Image.Height];

                for (int j = 0; j < Image.Height; j++)
                {
                    Color PixelColor = Image.GetPixel(j, i);

                    ImageColors[i][j] = PixelColor;
                    PreviewImageColors[i][j] = PixelColor;

                    if (PixelColor.ToArgb() == Color.White.ToArgb())
                    {
                        OpenNodes.Add(new Point(i, j));
                    }
                }
            }

            return OpenNodes;
        }

        private void InitializeNodes(List<Point> openNodes)
        {
            // Populate map
            foreach (Point openNode in openNodes)
            {
                MapNode Node = new MapNode()
                {
                    Position = openNode,
                    NodeValue = 0,
                };

                if (!Nodes.ContainsKey(openNode.X))
                {
                    Nodes[openNode.X] = new ConcurrentDictionary<int, MapNode>();
                }

                Nodes[openNode.X][openNode.Y] = Node;
            }

            // Set StartNode/EndNode
            StartNodePosition = Nodes[0].Values.First().Position;
            EndNodePosition = Nodes[Nodes.Count() - 1].Values.First().Position;

            // Build node relationships
            foreach (Point node in openNodes)
            {
                Point NeighborPosition = null;

                // North
                NeighborPosition = new Point(node.X - 1, node.Y);
                if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y))
                {
                    Nodes[node.X][node.Y].NorthNode = NeighborPosition;
                }

                // East
                NeighborPosition = new Point(node.X, node.Y + 1);
                if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y))
                {
                    Nodes[node.X][node.Y].EastNode = NeighborPosition;
                }

                // South
                NeighborPosition = new Point(node.X + 1, node.Y);
                if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y))
                {
                    Nodes[node.X][node.Y].SouthNode = NeighborPosition;
                }

                // West
                NeighborPosition = new Point(node.X, node.Y - 1);
                if (Nodes.ContainsKey(NeighborPosition.X) && Nodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y))
                {
                    Nodes[node.X][node.Y].WestNode = NeighborPosition;
                }
            }
        }

        public async Task InitializeAsync()
        {
            List<Point> OpenNodes = null;
            if (_SolutionPath != null)
            {
                OpenNodes = await Task.Run(() => { return _SolutionPath.Select(x => new Point(Convert.ToInt32(x.Split(',')[0]), Convert.ToInt32(x.Split(',')[1]))).ToList(); }).ConfigureAwait(false);
            }
            else
            {
                OpenNodes = await Task.Run(() => { return InitializeImageColors(); });
            }

            await Task.Run(() => InitializeNodes(OpenNodes));
        }
        #endregion Methods..
    }
}
