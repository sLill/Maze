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
        protected override bool CheckPathForExcursions()
        {
            return base.CheckPathForExcursions();
        }

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
            NodeQueue.Enqueue(Map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            // End Node
            MapNode EndNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            MapNode CurrentNode;
            while (NodeQueue.Count > 0)
            {
                CurrentNode = NodeQueue.Dequeue();

                CurrentNode.AppendPointToPath();
                CurrentNode.NodeValue = 2;

                // Push to preview buffer
                Map.PreviewPixelBuffer.Push(CurrentNode.Position);

                if (CurrentNode.Position == EndNode.Position)
                {
                    return true;
                }
                else
                {
                    if (CurrentNode.NorthNode != null && CurrentNode.NorthNode.NodeValue == 0)
                    {
                        CurrentNode.NorthNode.Path = CurrentNode.Path;
                        CurrentNode.NorthNode.NodeValue = 1;

                        CurrentNode.NorthNode.MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;
                        NodeQueue.Enqueue(CurrentNode.NorthNode);
                    }
                    if (CurrentNode.EastNode != null && CurrentNode.EastNode.NodeValue == 0)
                    {
                        CurrentNode.EastNode.Path = CurrentNode.Path;
                        CurrentNode.EastNode.NodeValue = 1;

                        CurrentNode.EastNode.MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;
                        NodeQueue.Enqueue(CurrentNode.EastNode);
                    }
                    if (CurrentNode.SouthNode != null && CurrentNode.SouthNode.NodeValue == 0)
                    {
                        CurrentNode.SouthNode.Path = CurrentNode.Path;
                        CurrentNode.SouthNode.NodeValue = 1;

                        CurrentNode.SouthNode.MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;
                        NodeQueue.Enqueue(CurrentNode.SouthNode);
                    }
                    if (CurrentNode.WestNode != null && CurrentNode.WestNode.NodeValue == 0)
                    {
                        CurrentNode.WestNode.Path = CurrentNode.Path;
                        CurrentNode.WestNode.NodeValue = 1;

                        CurrentNode.WestNode.MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;
                        NodeQueue.Enqueue(CurrentNode.WestNode);
                    }
                }

                CurrentNode.Dispose();
            }

            return base.Search();
        }

        #region Methods..
        public override bool Solve()
        {        
            return base.Solve();
        }
        #endregion Methods..
    }
}
