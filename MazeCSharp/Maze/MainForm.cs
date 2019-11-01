using MazeCommon;
using MazeTraversal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maze
{
    public partial class MainForm : Form, IDisposable
    {
        #region Member Variables..
        private static string _FileName = string.Empty;
        private static Bitmap _ImageMap;
        private static Color[][] _ImageColors;
        private static Map _Map;
        #endregion Member Variables..

        #region Properties..
        public string SelectedAlgorithm => cbAlgorithm.SelectedItem.ToString();
        #endregion Properties..

        #region Constructors..
        public MainForm()
        {
            InitializeComponent();
            InitializeControls();

            cbAlgorithm.SelectedIndex = 0;
        }

        #endregion Constructors..

        #region Methods..
        private void InitializeControls()
        {
            lblStatus.ForeColor = Color.Black;
            lblStatus.Text = "-";
            lblTimer.Text = "-";
            lblNodeCount.Text = "-";
        }

        private void ProcessNodes()
        {
            _Map = new Map(_ImageColors.Length, _ImageColors[0].Length);

            // Populate Map
            Parallel.For(0, _ImageColors.Length, (i) =>
            {
                for (int j = 0; j < _ImageColors[i].Length; j++)
                {
                    string Position = $"{i},{j}";

                    // 0 = White, -1 = Black
                    int ColorValue = _ImageColors[i][j] == Color.FromArgb(0, 0, 0) ? -1 : 0;
                    bool IsStartNode = i == 0 && ColorValue == 0;
                    bool IsEndNode = i == _ImageColors.Length - 1 && ColorValue == 0;

                    _Map.Nodes[Position] = new MapNode()
                    {
                        Position = new MazeCommon.Point() { X = j, Y = i },
                        NodeValue = ColorValue,
                        IsStartNode = IsStartNode,
                        IsEndNode = IsEndNode
                    };
                }
            });

            // Process Map and find all nodes
            Parallel.For(0, _ImageColors.Length, (i) =>
            {
                for (int j = 0; j < _ImageColors[i].Length; j++)
                {
                    string Position = $"{i},{j}";
                    string NeighborPosition = string.Empty;

                    // North
                    if (i != 0)
                    {
                        NeighborPosition = $"{i - 1},{j}";
                        if (_Map.Nodes[NeighborPosition].NodeValue == 0)
                        {
                            _Map.Nodes[Position].NorthNode = _Map.Nodes[NeighborPosition];
                        }
                    }

                    // West
                    if (j != 0)
                    {
                        NeighborPosition = $"{i},{j - 1}";
                        if (_Map.Nodes[NeighborPosition].NodeValue == 0)
                        {
                            _Map.Nodes[Position].WestNode = _Map.Nodes[NeighborPosition];
                        }
                    }

                    // South
                    if (i != _ImageColors.Length - 1)
                    {
                        NeighborPosition = $"{i + 1},{j}";
                        if (_Map.Nodes[NeighborPosition].NodeValue == 0)
                        {
                            _Map.Nodes[Position].SouthNode = _Map.Nodes[NeighborPosition];
                        }
                    }

                    // East
                    if (j != _ImageColors[i].Length - 1)
                    {
                        NeighborPosition = $"{i},{j + 1}";
                        if (_Map.Nodes[NeighborPosition].NodeValue == 0)
                        {
                            _Map.Nodes[Position].EastNode = _Map.Nodes[NeighborPosition];
                        }
                    }
                }
            });
        }

        private bool Solve()
        {
            bool MultithreadingEnabled = chkMultithreading.Checked;

            TraversalType TraversalType = null;
            switch (SelectedAlgorithm)
            {
                case "BreadthFirst":
                    TraversalType = new BreadthFirst(_Map);
                    break;
                case "DepthFirst":
                    TraversalType = new DepthFirst(_Map);
                    break;
                case "Astar":
                    break;
                case "Djikstra":
                    break;
                case "FibonacciHeap":
                    break;
                case "LeftTurn":
                    break;
                default:
                    break;
            }

            return TraversalType.Solve(MultithreadingEnabled);
        }

        private void ReColorMap()
        {
            if (cbOutputPath.Checked)
            {
                MapNode EndNode = _Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
                string[] Positions = EndNode.Path.Split(':');

                foreach (string position in Positions)
                {
                    if (position != string.Empty)
                    {
                        int X = Convert.ToInt32(position.Split(',')[0]);
                        int Y = Convert.ToInt32(position.Split(',')[1]);
                        _ImageColors[Y][X] = Color.Red;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, MapNode> node in _Map.Nodes)
                {
                    if (node.Value.NodeValue == 2)
                    {
                        string[] Position = node.Key.Split(',');
                        int X = Convert.ToInt32(Position[0]);
                        int Y = Convert.ToInt32(Position[1]);

                        _ImageColors[X][Y] = Color.Red;
                    }
                }
            }
        }

        private void SaveSolution()
        {
            string SavePath = Path.Combine(Environment.CurrentDirectory, "maze_solutions", $"{_FileName}_Solution.png");

            int RgbIndex = 0;
            byte[] RgbData = new byte[_ImageColors.Length * _ImageColors[0].Length * 4];
            for (int i = 0; i < _ImageColors.Length; i++)
            {
                for (int j = 0; j < _ImageColors[i].Length; j++)
                {
                    RgbData[RgbIndex++] = _ImageColors[i][j].B;
                    RgbData[RgbIndex++] = _ImageColors[i][j].G;
                    RgbData[RgbIndex++] = _ImageColors[i][j].R;
                    RgbData[RgbIndex++] = _ImageColors[i][j].A;
                }
            }

            Bitmap ImageBitmap = new Bitmap(_ImageColors[0].Length, _ImageColors.Length, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var ImageBitmapData = ImageBitmap.LockBits(new Rectangle(0, 0, ImageBitmap.Width, ImageBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, ImageBitmap.PixelFormat);
            Marshal.Copy(RgbData, 0, ImageBitmapData.Scan0, RgbData.Length);
            ImageBitmap.UnlockBits(ImageBitmapData);
            ImageBitmap.Save(SavePath);
        }

        private void SetStatus(Status status)
        {
            string StatusText = string.Empty;
            Color StatusColor = Color.Black;

            switch(status)
            {
                case Status.LoadingImage:
                    StatusText = "Loading image..";
                    break;
                case Status.InitializingNodes:
                    StatusText = "Initializing nodes..";
                    break;
                case Status.Solving:
                    StatusText = "Solving..";
                    break;
                case Status.Success:
                    StatusText = "Success";
                    StatusColor = Color.Green;
                    break;
                case Status.Failed:
                    StatusText = "Failed";
                    StatusColor = Color.Red;
                    break;
            }

            lblStatus.ForeColor = StatusColor;
            lblStatus.Text = StatusText;
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            InitializeControls();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetStatus(Status.LoadingImage);

                    _FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    string FilePath = Path.Combine(Environment.CurrentDirectory, openFileDialog.FileName);

                    _ImageMap = new Bitmap(FilePath);

                    _ImageColors = new Color[_ImageMap.Width][];
                    for (int i = 0; i < _ImageMap.Width; i++)
                    {
                        _ImageColors[i] = new Color[_ImageMap.Height];
                        for (int j = 0; j < _ImageMap.Height; j++)
                        {
                            _ImageColors[i][j] = _ImageMap.GetPixel(j, i);
                        }
                    }

                    ThreadSafeEventTimer TraversalTimer = ThreadSafeEventTimer.StartNew();

                    try
                    {
                        SetStatus(Status.InitializingNodes);
                        ProcessNodes();

                        lblNodeCount.Text = _Map.Nodes.Count(x => x.Value.NodeValue == 0).ToString();
                        SetStatus(Status.Solving);

                        bool SolveResult = await Task.Run(() => Solve());
                        ReColorMap();
                        SaveSolution();

                        Status SolveResultStatus = SolveResult ? Status.Success : Status.Failed;
                        SetStatus(SolveResultStatus);
                    }
                    catch (Exception ex)
                    {
                        SetStatus(Status.Failed);
                    }
                    finally
                    {
                        TraversalTimer.Stop();
                    }
                }
            }
        }
        #endregion Methods..
    }
}
