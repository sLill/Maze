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
        private static string _fileName = string.Empty;
        private static string _algorithm = string.Empty;
        private static Stopwatch _stopwatch;
        private static Bitmap _imageMap;
        private static Color[][] _imageColors;
        private static Map _map;

        public MainForm()
        {
            InitializeComponent();
            InitializeControls();

            cbAlgorithm.SelectedIndex = 0;
        }

        private void InitializeControls()
        {
            lblStatus.ForeColor = Color.Black;
            lblStatus.Text = "-";
            lblTimer.Text = "-";
            lblNodeCount.Text = "-";
        }

        private void ProcessNodes()
        {
            _map = new Map(_imageColors.Length, _imageColors[0].Length);

            // Populate Map
            Parallel.For(0, _imageColors.Length, (i) =>
            {
                for (int j = 0; j < _imageColors[i].Length; j++)
                {
                    string position = $"{i},{j}";

                    // 0 = White, -1 = Black
                    int val = _imageColors[i][j] == Color.FromArgb(0, 0, 0) ? -1 : 0;
                    bool isStartNode = i == 0 && val == 0;
                    bool isEndNode = i == _imageColors.Length - 1 && val == 0;

                    _map.Nodes[position] = new MapNode() {
                        Position = new Point() { X = j, Y = i },
                        NodeValue = val,
                        IsStartNode = isStartNode,
                        IsEndNode = isEndNode
                    };
                }
            });

            // Process Map and find all nodes
            Parallel.For(0, _imageColors.Length, (i) =>
            {
                for (int j = 0; j < _imageColors[i].Length; j++)
                {
                    string position = $"{i},{j}";
                    string neighborPos = string.Empty;

                    // North
                    if (i != 0)
                    {
                        neighborPos = $"{i - 1},{j}";
                        if (_map.Nodes[neighborPos].NodeValue == 0)
                        {
                            _map.Nodes[position].NorthNode = _map.Nodes[neighborPos];
                            //_map.Nodes[position].ConnectedNodes++;
                        }
                    }

                    // West
                    if (j != 0)
                    {
                        neighborPos = $"{i},{j - 1}";
                        if (_map.Nodes[neighborPos].NodeValue == 0)
                        {
                            _map.Nodes[position].WestNode = _map.Nodes[neighborPos];
                            //_map.Nodes[position].ConnectedNodes++;
                        }
                    }

                    // South
                    if (i != _imageColors.Length - 1)
                    {
                        neighborPos = $"{i + 1},{j}";
                        if (_map.Nodes[neighborPos].NodeValue == 0)
                        {
                            _map.Nodes[position].SouthNode = _map.Nodes[neighborPos];
                            //_map.Nodes[position].ConnectedNodes++;
                        }
                    }

                    // East
                    if (j != _imageColors[i].Length - 1)
                    {
                        neighborPos = $"{i},{j + 1}";
                        if (_map.Nodes[neighborPos].NodeValue == 0)
                        {
                            _map.Nodes[position].EastNode = _map.Nodes[neighborPos];
                            //_map.Nodes[position].ConnectedNodes++;
                        }
                    }
                }
            });
        }

        private void ProcessRoute()
        {
            bool useMultithreading = chkMultithreading.Checked;

            switch (_algorithm)
            {
                case "BreadthFirst":
                    if (useMultithreading)
                        Solve.BreadthFirstMulti(_map);
                    else
                        Solve.BreadthFirst(_map);
                    break;
                case "DepthFirst":
                    Solve.DepthFirst(_map);
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
        }

        private void ReColorMap()
        {
            if (cbOutputPath.Checked)
            {
                MapNode endNode = _map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
                string[] positions = endNode.Path.Split(':');

                foreach (string position in positions)
                {
                    if (position != string.Empty)
                    {
                        int x = Convert.ToInt32(position.Split(',')[0]);
                        int y = Convert.ToInt32(position.Split(',')[1]);
                        _imageColors[y][x] = Color.Red;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, MapNode> node in _map.Nodes)
                {
                    if (node.Value.NodeValue == 2)
                    {
                        string[] position = node.Key.Split(',');
                        int x = Convert.ToInt32(position[0]);
                        int y = Convert.ToInt32(position[1]);

                        _imageColors[x][y] = Color.Red;
                    }
                }
            }
        }

        private void SaveSolution()
        {
            string savePath = Path.Combine(Environment.CurrentDirectory, "maze_solutions", $"{_fileName}_Solution.png");
            //_imageMap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);

            int rgbIndex = 0;
            byte[] rgbData = new byte [_imageColors.Length * _imageColors[0].Length * 4];
            for (int i = 0; i < _imageColors.Length; i++)
            {
                for (int j = 0; j < _imageColors[i].Length; j++)
                {
                    rgbData[rgbIndex++] = _imageColors[i][j].B;
                    rgbData[rgbIndex++] = _imageColors[i][j].G;
                    rgbData[rgbIndex++] = _imageColors[i][j].R;
                    rgbData[rgbIndex++] = _imageColors[i][j].A;
                }
            }

            Bitmap bmp = new Bitmap(_imageColors[0].Length, _imageColors.Length, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            Marshal.Copy(rgbData, 0, bitmapData.Scan0, rgbData.Length);
            bmp.UnlockBits(bitmapData);
            bmp.Save(savePath);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            _algorithm = cbAlgorithm.SelectedItem.ToString();

            InitializeControls();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                lblStatus.Text = "Loading image..";

                _fileName = Path.GetFileNameWithoutExtension(dialog.FileName);
                string filePath = Path.Combine(Environment.CurrentDirectory, dialog.FileName);

                _imageMap = new Bitmap(filePath);

                _imageColors = new Color[_imageMap.Width][];
                for(int i = 0; i < _imageMap.Width; i++)
                {
                    _imageColors[i] = new Color[_imageMap.Height];
                    for (int j = 0; j < _imageMap.Height; j++)
                    {
                        _imageColors[i][j] = _imageMap.GetPixel(j, i);
                    }
                }

                _stopwatch = new Stopwatch();
                _stopwatch.Start();

                try
                {
                    lblStatus.Text = "Processing nodes..";
                    ProcessNodes();

                    lblNodeCount.Text = _map.Nodes.Count(x => x.Value.NodeValue == 0).ToString();

                    lblStatus.Text = "Processing route..";
                    ProcessRoute();
                    ReColorMap();

                    SaveSolution();

                    // Update timer
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblTimer.Text = _stopwatch.Elapsed.ToString("mm\\:ss\\.ff");
                    });

                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Success";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Failed";
                }

                _stopwatch.Stop();
            }
        }
    }
}
