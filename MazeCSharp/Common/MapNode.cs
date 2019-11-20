﻿using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;

namespace Common
{
    public class MapNode : IDisposable
    {
        #region Properties..
        public MapNode NorthNode { get; set; }

        public MapNode EastNode { get; set; }

        public MapNode SouthNode { get; set; }

        public MapNode WestNode { get; set; }

        public Point Position { get; set; }

        public bool IsEndNode { get; set; }

        public bool IsStartNode { get; set; }

        public MemoryMappedFileManager MemoryMappedFileManager { get; set; }

        public object NodeLock { get; set; }

        // -1 = Wall, 0 = Path (unvisited), 1 = Path (visited), 2 = Path (visited and examined)
        public int NodeValue { get; set; }

        public string Path { get; set; }
        #endregion Properties..

        #region Constructors..
        public MapNode()
        {
            Path = string.Empty;
            NodeLock = new object();
        }
        #endregion Constructors..

        #region Methods..
        public void AppendPointToPath()
        {
            // 50 Mb files
            if (Path.Length >= 50000)
            {
                MemoryMappedFileManager = MemoryMappedFileManager ?? new MemoryMappedFileManager();

                MemoryMappedFileManager.CreateNewMappedFile(this.Path);
                Path = string.Empty;
            }

            Path += $":{this.Position.X.ToString()},{this.Position.Y.ToString()}";
        }

        public void Dispose()
        {
            Path = null;
        }

        /// <summary>
        /// Retrieves the next path segment from the stack as a string. 
        /// Returns null if none exist.
        /// </summary>
        /// <returns></returns>
        public bool GetPathSegment(out string pathSegment)
        {
            pathSegment = string.Empty;
            if (Path != string.Empty)
            {
                pathSegment = Path;
                Path = string.Empty;
            }
            else
            {
                pathSegment = MemoryMappedFileManager.GetFileContent() ?? string.Empty;
            }

            return pathSegment != string.Empty;
        }
        #endregion Methods..
    }
}
