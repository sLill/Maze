using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implementation
{
    public class BreadthFirst : TraversalType
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public BreadthFirst(Map map)
            : base(map) { }
        #endregion Constructors..

        #region Methods..
        protected override bool Search()
        {
            //           Breadth - First - Search(Maze m)
            //              EnQueue(m.StartNode)
            //              While Queue.NotEmpty
            //                  c < -DeQueue
            //                  If c is the goal
            //                      Exit
            //                  Else
            //                     Foreach neighbor n of c
            //                          If n "Unvisited"
            //                              Mark n "Visited"
            //                              EnQueue(n)
            //                     Mark c "Examined"
            //            End procedure

            Queue<MapNode> NodeQueue = new Queue<MapNode>();

            // Start Node
            NodeQueue.Enqueue(Map.StartNode);

            // End Node
            MapNode CurrentNode;
            while (NodeQueue.Count > 0)
            {
                CurrentNode = NodeQueue.Dequeue();
                CurrentNode.AppendPointToPath();
                CurrentNode.NodeValue = 2;

                // Push to preview buffer
                Map.PreviewPixelBuffer.Push(CurrentNode.Position);

                if (CurrentNode.Position == Map.EndNode.Position)
                {
                    return true;
                }
                else
                {
                    List<Point> NeighborPositions = new List<Point>()
                    {
                        CurrentNode.NorthNode,
                        CurrentNode.EastNode,
                        CurrentNode.SouthNode,
                        CurrentNode.WestNode
                    };

                    NeighborPositions.RemoveAll(x => x == null);

                    foreach (Point neighborPosition in NeighborPositions)
                    {
                        if (neighborPosition != null && Map.Nodes.ContainsKey(neighborPosition.X) && Map.Nodes[neighborPosition.X].ContainsKey(neighborPosition.Y)
                            && Map.Nodes[neighborPosition.X][neighborPosition.Y].NodeValue == 0)
                        {
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].Path = CurrentNode.Path;
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].NodeValue = 1;
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeQueue.Enqueue(Map.Nodes[neighborPosition.X][neighborPosition.Y]);
                        }
                    }

                    CurrentNode.Dispose();
                    CurrentNode = null;
                }
            }


            return base.Search();
        }

        #endregion Methods..
    }
}
