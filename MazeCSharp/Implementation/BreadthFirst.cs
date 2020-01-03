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
                    string NorthNodePosition = CurrentNode.NorthNode;
                    string EastNodePosition = CurrentNode.EastNode;
                    string SouthNodePosition = CurrentNode.SouthNode;
                    string WestNodePosition = CurrentNode.WestNode;

                    try
                    {
                        if (NorthNodePosition != null && Map.Nodes.ContainsKey(NorthNodePosition) && Map.Nodes[NorthNodePosition].NodeValue == 0)
                        {
                            Map.Nodes[NorthNodePosition].Path = CurrentNode.Path;
                            Map.Nodes[NorthNodePosition].NodeValue = 1;
                            Map.Nodes[NorthNodePosition].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeQueue.Enqueue(Map.Nodes[NorthNodePosition]);
                        }
                        if (EastNodePosition != null && Map.Nodes.ContainsKey(EastNodePosition) && Map.Nodes[EastNodePosition].NodeValue == 0)
                        {
                            Map.Nodes[EastNodePosition].Path = CurrentNode.Path;
                            Map.Nodes[EastNodePosition].NodeValue = 1;
                            Map.Nodes[EastNodePosition].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeQueue.Enqueue(Map.Nodes[EastNodePosition]);
                        }
                        if (SouthNodePosition != null && Map.Nodes.ContainsKey(SouthNodePosition) && Map.Nodes[SouthNodePosition].NodeValue == 0)
                        {
                            Map.Nodes[SouthNodePosition].Path = CurrentNode.Path;
                            Map.Nodes[SouthNodePosition].NodeValue = 1;
                            Map.Nodes[SouthNodePosition].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeQueue.Enqueue(Map.Nodes[SouthNodePosition]);
                        }
                        if (WestNodePosition != null && Map.Nodes.ContainsKey(WestNodePosition) && Map.Nodes[WestNodePosition].NodeValue == 0)
                        {
                            Map.Nodes[WestNodePosition].Path = CurrentNode.Path;
                            Map.Nodes[WestNodePosition].NodeValue = 1;
                            Map.Nodes[WestNodePosition].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeQueue.Enqueue(Map.Nodes[WestNodePosition]);
                        }

                        CurrentNode.Dispose();
                        CurrentNode = null;
                    }
                    catch (Exception ex)
                    { }
                }
            }

            return base.Search();
        }

        #endregion Methods..
    }
}
