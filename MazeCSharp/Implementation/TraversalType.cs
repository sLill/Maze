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
        private void MarkDeadEnd(ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>> mapNodes, Point nodePosition)
        {
            MapNode Node = null;
            if (mapNodes.ContainsKey(nodePosition.X) && mapNodes[nodePosition.X].ContainsKey(nodePosition.Y)
                && mapNodes[nodePosition.X][nodePosition.Y].ConnectedNodes < 2)
            {
                mapNodes[nodePosition.X].TryRemove(nodePosition.Y, out Node);
            }

            if (Node != null)
            {
                if (Node.NorthNode != null)
                {
                    MarkDeadEnd(mapNodes, Node.Position);
                }

                if (Node.SouthNode != null)
                {
                    MarkDeadEnd(mapNodes, Node.Position);
                }

                if (Node.EastNode != null)
                {
                    MarkDeadEnd(mapNodes, Node.Position);
                }

                if (Node.WestNode != null)
                {
                    MarkDeadEnd(mapNodes, Node.Position);
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

            List<string> PathSegments = Map.EndNode.GetPathSegments();

            if (PathSegments.Count > 1)
            {
                var RevisedNodes = new ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>>();

                List<string> FullPath = string.Join(string.Empty, PathSegments).Replace("\0", string.Empty).Split(':').ToList();
                FullPath.RemoveAll(x => x == string.Empty);

                // Build localized map
                for (int i = 0; i < FullPath.Count; i++)
                {
                    Point NodePosition = new Point(Convert.ToInt32(FullPath[i].Split(',')[0]), Convert.ToInt32(FullPath[i].Split(',')[1]));

                    MapNode Node = null;
                    if (RevisedNodes.ContainsKey(NodePosition.X) && RevisedNodes[NodePosition.X].ContainsKey(NodePosition.Y))
                    {
                        Node = RevisedNodes[NodePosition.X][NodePosition.Y];
                    }
                    else
                    {
                        Node = new MapNode()
                        {
                            Position = NodePosition,
                            NodeValue = 0
                        };

                        if (!RevisedNodes.ContainsKey(NodePosition.X))
                        {
                            RevisedNodes[NodePosition.X] = new ConcurrentDictionary<int, MapNode>();
                        }

                        RevisedNodes[NodePosition.X][NodePosition.Y] = Node;
                    }
                }

                // Build node relationships
                foreach (var row in RevisedNodes)
                {
                    foreach (var column in RevisedNodes[row.Key])
                    {
                        Point NeighborPosition = null;

                        // North
                        if (row.Key != 0)
                        {
                            NeighborPosition = new Point(row.Key - 1, column.Key);
                            if (RevisedNodes.ContainsKey(NeighborPosition.X) && RevisedNodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && RevisedNodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                RevisedNodes[row.Key][column.Key].NorthNode = NeighborPosition;
                            }
                        }

                        // East
                        if (column.Key != RevisedNodes[row.Key].Count - 1)
                        {
                            NeighborPosition = new Point(row.Key, column.Key + 1);
                            if (RevisedNodes.ContainsKey(NeighborPosition.X) && RevisedNodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && RevisedNodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                RevisedNodes[row.Key][column.Key].EastNode = NeighborPosition;
                            }
                        }

                        // South
                        if (row.Key != RevisedNodes[row.Key].Count - 1)
                        {
                            NeighborPosition = new Point(row.Key + 1, column.Key);
                            if (RevisedNodes.ContainsKey(NeighborPosition.X) && RevisedNodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && RevisedNodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                RevisedNodes[row.Key][column.Key].SouthNode = NeighborPosition;
                            }
                        }

                        // West
                        if (column.Key != 0)
                        {
                            NeighborPosition = new Point(row.Key, column.Key - 1);
                            if (RevisedNodes.ContainsKey(NeighborPosition.X) && RevisedNodes[NeighborPosition.X].ContainsKey(NeighborPosition.Y)
                                && RevisedNodes[NeighborPosition.X][NeighborPosition.Y].NodeValue == 0)
                            {
                                RevisedNodes[row.Key][column.Key].WestNode = NeighborPosition;
                            }
                        }
                    }
                }

                // Find all dead end nodes and remove their paths
                foreach (var row in RevisedNodes)
                {
                    var DeadEndNodes = new Stack<MapNode>(RevisedNodes[row.Key].Where(x => x.Value.ConnectedNodes == 1 && !x.Value.IsStartNode && !x.Value.IsEndNode).Select(x => x.Value));
                    while (DeadEndNodes.Count() > 0)
                    {
                        MarkDeadEnd(RevisedNodes, DeadEndNodes.Pop().Position);
                    }
                };

                Map.Nodes = RevisedNodes;
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
