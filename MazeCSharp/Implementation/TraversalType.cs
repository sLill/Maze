using Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private void MarkDeadEnd(MapNode node)
        {
            node.NodeValue = -1;
            node.ConnectedNodes.ForEach(connectedNode =>
            {
                if (connectedNode.ConnectedNodes.Count() < 3)
                {
                    MarkDeadEnd(connectedNode);
                }
            });
        }

        /// <summary>
        /// Returns true if the solution path contains any nodes with more than two connections
        /// </summary>
        /// <returns></returns>
        protected virtual bool RemoveExcursions()
        {
            bool Result = false;
            int NodeCount = Map.Nodes.Count;

            MapNode StartNode = Map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value;
            MapNode EndNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            List<string> PathSegments = EndNode.GetPathSegments();

            if (PathSegments.Count > 1)
            {
                Map SanitizedMap = new Map();

                // Start with the last segment (index 0) and work backwards
                List<string> FullPath = string.Join(string.Empty, PathSegments).Split(':').ToList();
                FullPath.RemoveAll(x => x == string.Empty);

                // Build localized map
                for (int i = 0; i < FullPath.Count; i++)
                {
                    MapNode Node = null;
                    if (SanitizedMap.Nodes.ContainsKey(FullPath[i]))
                    {
                        Node = SanitizedMap.Nodes[FullPath[i]];
                    }
                    else
                    {
                        Node = new MapNode()
                        {
                            Position = new Point() { X = Convert.ToInt32(FullPath[i].Split(',')[0]), Y = Convert.ToInt32(FullPath[i].Split(',')[1]) },
                            NodeValue = 0
                        };

                        SanitizedMap.Nodes[FullPath[i]] = Node;
                    }
                }

                // Find/Set neighbors
                Parallel.ForEach(SanitizedMap.Nodes.ToList(), pivotNode =>
                {
                    var PivotNeighbors = SanitizedMap.Nodes.Where(x => (x.Value.Position - pivotNode.Value.Position) == 1).Select(x => x.Value).ToList();

                    pivotNode.Value.EastNode = PivotNeighbors.Where(x => x.Position.Y > pivotNode.Value.Position.Y).FirstOrDefault();
                    pivotNode.Value.WestNode = PivotNeighbors.Where(x => x.Position.Y < pivotNode.Value.Position.Y).FirstOrDefault();
                    pivotNode.Value.NorthNode = PivotNeighbors.Where(x => x.Position.X < pivotNode.Value.Position.X).FirstOrDefault();
                    pivotNode.Value.SouthNode = PivotNeighbors.Where(x => x.Position.X > pivotNode.Value.Position.X).FirstOrDefault();
                });

                // Set Start/End Nodes 
                SanitizedMap.Nodes[StartNode.Position.ToString()].IsStartNode = true;
                SanitizedMap.Nodes[EndNode.Position.ToString()].IsEndNode = true;

                // Find all dead end nodes and remove their paths
                var DeadEndNodes = new Stack<MapNode>(SanitizedMap.Nodes.Where(x => x.Value.ConnectedNodes.Count == 1 && !x.Value.IsStartNode && !x.Value.IsEndNode).Select(x => x.Value));
                while (DeadEndNodes.Count() > 0)
                {
                    MarkDeadEnd(DeadEndNodes.Pop());
                }

                Map = null;
                Map = SanitizedMap;
            }

            Result = Map.Nodes.Count == NodeCount;
            return !Result;
        }

        protected virtual bool Search() { return true; }

        public virtual bool Solve()
        {
            bool Solved = false;
            while (!Solved)
            {
                this.Search();

                // Very large mazes cache their pathing in memory as unfinished segments using MemoryMappedFiles. When this happens, it
                // becomes necessary to work backwards again through the solution again
                Solved = !RemoveExcursions();
            }

            this.Search();
            return true;
        }
        #endregion Methods..
    }
}
