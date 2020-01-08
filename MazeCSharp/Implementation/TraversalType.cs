using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Implementation
{
    public abstract class TraversalType
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public Map Map { get; private set; }
        #endregion Properties..

        #region Constructors..
        public TraversalType(Map map)
        {
            Map = map;
        }
        #endregion Constructors..

        #region Methods..
        private void RemoveDeadEnd(ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>> mapNodes, Point nodePosition)
        {
            if (nodePosition != null)
            {
                if (mapNodes.ContainsKey(nodePosition.X) && mapNodes[nodePosition.X].ContainsKey(nodePosition.Y)
                    && mapNodes[nodePosition.X][nodePosition.Y].ConnectedNodes < 3)
                {
                    mapNodes[nodePosition.X].TryRemove(nodePosition.Y, out MapNode Node);

                    RemoveDeadEnd(mapNodes, Node.NorthNode);
                    RemoveDeadEnd(mapNodes, Node.EastNode);
                    RemoveDeadEnd(mapNodes, Node.SouthNode);
                    RemoveDeadEnd(mapNodes, Node.WestNode);
                }
            }
        }


        /// <summary>
        /// Returns true if the solution path contains any nodes with more than two connections
        /// </summary>
        /// <returns></returns>
        protected virtual bool RemoveExcursions()
        {
            bool Result = false;
            int NodeCount = Map.Nodes.Sum(x => x.Value.Count);

            List<string> PathSegments = Map.Nodes[Map.EndNodePosition.X][Map.EndNodePosition.Y].GetPathSegments();

            if (PathSegments.Count > 1)
            {
                List<string> FullPath = string.Join(string.Empty, PathSegments).Replace("\0", string.Empty).Split(':').ToList();
                FullPath.RemoveAll(x => x == string.Empty);

                Map RevisedMap = new Map(FullPath);

                // Find all dead end nodes and remove their paths
                foreach (var row in RevisedMap.Nodes)
                {
                    var DeadEndNodes = new Stack<MapNode>(RevisedMap.Nodes[row.Key].Where(x => x.Value.ConnectedNodes == 1 
                        && x.Value.Position.ToString() != Map.StartNodePosition.ToString() 
                        && x.Value.Position.ToString() != Map.EndNodePosition.ToString()).Select(x => x.Value));

                    while (DeadEndNodes.Count() > 0)
                    {
                        RemoveDeadEnd(RevisedMap.Nodes, DeadEndNodes.Pop().Position);
                    }
                };

                Map.Nodes = RevisedMap.Nodes;
            }

            Result = Map.Nodes.Sum(x => x.Value.Count) == NodeCount;
            return !Result;
        }

        protected virtual bool Search() { return true; }

        public virtual bool Solve()
        {
            bool Solved = false;
            while (!Solved)
            {
                this.Search();

                // Very large mazes cache their pathing in memory as unfinished segments using MemoryMappedFiles
                // When this happens, the mze must be rebuilt from the fragmented solution and processed again
                Solved = !RemoveExcursions();
            }

            return true;
        }
        #endregion Methods..
    }
}
