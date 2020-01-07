using Common;
using Implementation;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Maze
{
    public partial class MainForm : Form, IDisposable
    {
        #region Member Variables..
        private static string _FileName = string.Empty;
        private static Map _Map;
        private static ThreadSafeEventTimer _MazeTimer;
        private Task<Image> _RenderPreviewTask = null;
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Delegates..
        private delegate void OnTimerElapsed();
        #endregion Delegates..

        #region Constructors..
        public MainForm()
        {
            InitializeComponent();
            InitializeControls();

            cmbAlgorithm.SelectedIndex = 0;
        }

        #endregion Constructors..

        #region Methods..
        #region Event Handlers..
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            InitializeControls();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bool SolveResult = false;

                    SetStatus(Status.LoadingImage);

                    _FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    string FilePath = Path.Combine(Environment.CurrentDirectory, openFileDialog.FileName);

                    bool FloodFill = !cbOutputPath.Checked;
                    string SelectedAlgorithm = cmbAlgorithm.SelectedItem.ToString();

                    try
                    {
                        SetStatus(Status.Initializing);
                        _Map = new Map(new Bitmap(FilePath));
                        pbMaze.Image = new Bitmap(_Map.Image);

                        await Task.Run(() => _Map.InitializeImageColors()).ConfigureAwait(false);
                        await Task.Run(() => _Map.InitializeNodes()).ConfigureAwait(false);

                        _MazeTimer = ThreadSafeEventTimer.StartNew();
                        _MazeTimer.Elapsed += Timer_Elapsed;

                        SetStatus(Status.Solving);
                        SolveResult = await SolveAsync(SelectedAlgorithm);

                        _Map.DrawSolution(FloodFill);

                        Status SolveResultStatus = SolveResult ? Status.Success : Status.Failed;
                        SetStatus(SolveResultStatus);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.StackTrace, ex.Message);
                        SetStatus(Status.Failed);
                    }
                    finally
                    {
                        _MazeTimer.Stop();
                    }

                    Timer_Elapsed(_MazeTimer, null);

                    if (SolveResult)
                    {
                        _Map.ExportSolution(_FileName, SelectedAlgorithm, _MazeTimer.ElapsedMilliseconds);
                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Invoke(new OnTimerElapsed(async () =>
            {
                // Timer
                TimeSpan ElapsedTime = TimeSpan.FromMilliseconds(((ThreadSafeEventTimer)sender).ElapsedMilliseconds);
                lblTimer.Text = ElapsedTime.ToString();

                // Solution preview
                if (_RenderPreviewTask == null || _RenderPreviewTask.IsCompleted)
                {
                    _RenderPreviewTask = Task.Run(() => _Map.DrawPreview());
                    pbMaze.Image = await _RenderPreviewTask;
                }
            }));
        }
        #endregion Event Handlers..

        private void InitializeControls()
        {
            lblStatus.ForeColor = Color.Black;
            lblStatus.Text = "-";
            lblTimer.Text = "-";
            lblNodeCount.Text = "-";
        }

        private async Task<bool> SolveAsync(string selectedAlgorithm)
        {
            TraversalType TraversalType = null;
            switch (selectedAlgorithm)
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

            return await Task.Run(() => TraversalType.Solve());
        }

        private void SetStatus(Status status)
        {
            Invoke(new MethodInvoker(delegate
            {
                string StatusText = string.Empty;
                Color StatusColor = Color.Black;

                switch (status)
                {
                    case Status.LoadingImage:
                        StatusText = "Loading image..";
                        break;
                    case Status.Initializing:
                        StatusText = "Initializing..";
                        break;
                    case Status.Solving:
                        StatusText = "Solving..";
                        lblNodeCount.Text = _Map.Nodes.Sum(x => x.Value.Count).ToString();
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

                lblTimer.ForeColor = StatusColor;
                lblStatus.ForeColor = StatusColor;
                lblStatus.Text = StatusText;

                bool LockUI = (status == Status.Solving || status == Status.Initializing);
                cmbAlgorithm.Enabled = !LockUI;
                cbOutputPath.Enabled = status != Status.Solving;
            }));
        }
        #endregion Methods..
    }
}
