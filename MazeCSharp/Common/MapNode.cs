using System;
using System.Collections.Generic;

namespace Common
{
    public class MapNode : IDisposable
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public string NorthNode { get; set; }

        public string EastNode { get; set; }

        public string SouthNode { get; set; }

        public string WestNode { get; set; }

        public Point Position { get; set; }

        public bool IsEndNode { get; set; }

        public bool IsStartNode { get; set; }

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

        public string Path { get; set; }
        #endregion Properties..

        #region Constructors..
        public MapNode()
        {
            Path = string.Empty;
            PathSegments = new List<string>();
        }
        #endregion Constructors..

        #region Methods..
        public void AppendPointToPath()
        {
            // 5 Kb files
            if (Path.Length >= 50000)
            {
                MemoryMappedFileManager = MemoryMappedFileManager ?? new MemoryMappedFileManager();
                MemoryMappedFileManager.CreateNewMappedFile(this.Path);
                Path = string.Empty;
            }

            Path += $":{this.Position.ToString()}";
        }

        public void Dispose()
        {
            Path = string.Empty;
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
            if (Path != string.Empty)
            {
                PathSegments.Add(Path);
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
