using Common;
using System;
using System.Collections.Concurrent;
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
        private void MarkDeadEnd(ConcurrentDictionary<string, MapNode> mapNodes, MapNode node)
        {
            List<string> ConnectedNodes = new List<string>()
            {
                node.NorthNode,
                node.EastNode,
                node.SouthNode,
                node.WestNode
            };

            mapNodes.TryRemove(node.Position.ToString(), out MapNode removedNode);

            node.Dispose();
            node = null;

            ConnectedNodes.RemoveAll(x => x == null);
            ConnectedNodes.ForEach(connectedNode =>
            {
                if (mapNodes.ContainsKey(connectedNode) && mapNodes[connectedNode]?.ConnectedNodes < 3)
                {
                    MarkDeadEnd(mapNodes, mapNodes[connectedNode]);
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
                var RevisedNodes = new ConcurrentDictionary<string, MapNode>();

                // Start with the last segment (index 0) and work backwards
                List<string> FullPath = string.Join(string.Empty, PathSegments).Split(':').ToList();
                FullPath.RemoveAll(x => x == string.Empty);

                // Build localized map
                for (int i = 0; i < FullPath.Count; i++)
                {
                    MapNode Node = null;
                    if (RevisedNodes.ContainsKey(FullPath[i]))
                    {
                        Node = RevisedNodes[FullPath[i]];
                    }
                    else
                    {
                        Node = new MapNode()
                        {
                            Position = new Point() { X = Convert.ToInt32(FullPath[i].Split(',')[0]), Y = Convert.ToInt32(FullPath[i].Split(',')[1]) },
                            NodeValue = 0
                        };

                        RevisedNodes[FullPath[i]] = Node;
                    }
                }

                // Find/Set neighbors
                Parallel.ForEach(RevisedNodes.ToList(), pivotNode =>
                {
                    var PivotNeighbors = RevisedNodes.Where(x => (x.Value.Position - pivotNode.Value.Position) == 1).Select(x => x.Value).ToList();

                    pivotNode.Value.EastNode = PivotNeighbors.Where(x => x.Position.Y > pivotNode.Value.Position.Y).FirstOrDefault()?.Position.ToString();
                    pivotNode.Value.WestNode = PivotNeighbors.Where(x => x.Position.Y < pivotNode.Value.Position.Y).FirstOrDefault()?.Position.ToString();
                    pivotNode.Value.NorthNode = PivotNeighbors.Where(x => x.Position.X < pivotNode.Value.Position.X).FirstOrDefault()?.Position.ToString();
                    pivotNode.Value.SouthNode = PivotNeighbors.Where(x => x.Position.X > pivotNode.Value.Position.X).FirstOrDefault()?.Position.ToString();

                    //Point PivotNodePostion = pivotNode.Value.Position;

                    //string NorthNodeKey = $"{PivotNodePostion.X - 1},{PivotNodePostion.Y}";
                    //string EastNodeKey = $"{PivotNodePostion.X},{PivotNodePostion.Y + 1}";
                    //string SouthNodeKey = $"{PivotNodePostion.X + 1},{PivotNodePostion.Y}";
                    //string WestNodeKey = $"{PivotNodePostion.X},{PivotNodePostion.Y - 1}";

                    //pivotNode.Value.NorthNode = RevisedNodes.Keys.Contains(NorthNodeKey) ? RevisedNodes[NorthNodeKey] : null;
                    //pivotNode.Value.EastNode = RevisedNodes.Keys.Contains(EastNodeKey) ? RevisedNodes[EastNodeKey] : null;
                    //pivotNode.Value.SouthNode = RevisedNodes.Keys.Contains(SouthNodeKey) ? RevisedNodes[SouthNodeKey] : null;
                    //pivotNode.Value.WestNode = RevisedNodes.Keys.Contains(WestNodeKey) ? RevisedNodes[WestNodeKey] : null;
                });

                // Set Start/End Nodes 
                RevisedNodes[StartNode.Position.ToString()].IsStartNode = true;
                RevisedNodes[EndNode.Position.ToString()].IsEndNode = true;
                RevisedNodes[EndNode.Position.ToString()].PathSegments = EndNode.PathSegments;

                // Find all dead end nodes and remove their paths
                var DeadEndNodes = new Stack<MapNode>(RevisedNodes.Where(x => x.Value.ConnectedNodes == 1 && !x.Value.IsStartNode && !x.Value.IsEndNode).Select(x => x.Value));
                while (DeadEndNodes.Count() > 0)
                {
                    MarkDeadEnd(RevisedNodes, DeadEndNodes.Pop());
                }

                Map.Nodes = RevisedNodes;
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
