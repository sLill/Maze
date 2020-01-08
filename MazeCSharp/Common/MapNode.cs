using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class MapNode : IDisposable
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public Point NorthNode { get; set; }

        public Point EastNode { get; set; }

        public Point SouthNode { get; set; }

        public Point WestNode { get; set; }

        public Point Position { get; set; }

        public bool IsStartNode { get; set; }

        public bool IsEndNode { get; set; }

        public List<string> PathSegments { get; set; }

        public MemoryMappedFileManager MemoryMappedFileManager { get; set; }

        // -1 = Wall, 0 = Path (unvisited), 1 = Path (visited), 2 = Path (visited and examined)
        public int NodeValue { get; set; }

        public int ConnectedNodes
        {
            get
            {
                return (NorthNode == null ? 0 : 1) +
                       (EastNode == null ? 0 : 1) +
                       (SouthNode == null ? 0 : 1) +
                       (WestNode == null ? 0 : 1);
            }
        }

        public StringBuilder Path { get; set; }
        #endregion Properties..

        #region Constructors..
        public MapNode()
        {
            PathSegments = new List<string>();
        }
        #endregion Constructors..

        #region Methods..
        public void AppendPointToPath()
        {
            Path = Path ?? new StringBuilder();

            // 5 Kb files
            if (Path.Length >= 50000)
            {
                MemoryMappedFileManager = MemoryMappedFileManager ?? new MemoryMappedFileManager();
                MemoryMappedFileManager.CreateNewMappedFile(this.Path.ToString());
                Path = new StringBuilder();
            }

            Path.Append($":{this.Position.ToString()}");
        }

        public void Dispose()
        {
            Path = null;
            MemoryMappedFileManager = null;
            PathSegments = null;
        }

        /// <summary>
        /// Retrieves the next path segment from the stack as a string. 
        /// Returns null if none exist.
        /// </summary>
        /// <returns></returns>
        public List<string> GetPathSegments()
        {
            if (Path != null)
            {
                PathSegments.Add(Path.ToString());
            }

            if (MemoryMappedFileManager != null)
            {
                PathSegments.AddRange(MemoryMappedFileManager.GetFileContent());
            }

            return PathSegments;
        }
        #endregion Methods..
    }
}
