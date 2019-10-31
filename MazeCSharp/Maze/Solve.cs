using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maze
{
    public static class Solve
    {
        public static void BreadthFirst(Map map)
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

            Queue<MapNode> nodeQueue = new Queue<MapNode>();

            // Start Node
            nodeQueue.Enqueue(map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            // End Node
            MapNode endNode = map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            MapNode currNode;
            while (nodeQueue.Count > 0)
            { 
                currNode = nodeQueue.Dequeue();
                currNode.Path += ($":{currNode.Position.X.ToString()},{currNode.Position.Y.ToString()}");
                currNode.NodeValue = 2;

                if (currNode.Position == endNode.Position)
                {
                    return;
                }
                else
                {
                    if (currNode.NorthNode != null && currNode.NorthNode.NodeValue == 0)
                    {
                        currNode.NorthNode.Path = currNode.Path;
                        currNode.NorthNode.NodeValue = 1;

                        nodeQueue.Enqueue(currNode.NorthNode);
                    }
                    if (currNode.EastNode != null && currNode.EastNode.NodeValue == 0)
                    {
                        currNode.EastNode.Path = currNode.Path;
                        currNode.EastNode.NodeValue = 1;

                        nodeQueue.Enqueue(currNode.EastNode);
                    }
                    if (currNode.SouthNode != null && currNode.SouthNode.NodeValue == 0)
                    {
                        currNode.SouthNode.Path = currNode.Path;
                        currNode.SouthNode.NodeValue = 1;

                        nodeQueue.Enqueue(currNode.SouthNode);
                    }
                    if (currNode.WestNode != null && currNode.WestNode.NodeValue == 0)
                    {
                        currNode.WestNode.Path = currNode.Path;
                        currNode.WestNode.NodeValue = 1;

                        nodeQueue.Enqueue(currNode.WestNode);
                    }
                }

                // Free resources
                currNode.Path = null;
            }

            return;
        }

        public static void BreadthFirstMulti(Map map)
        {
            ConcurrentQueue<MapNode> nodeQueue = new ConcurrentQueue<MapNode>();

            // Start Node
            nodeQueue.Enqueue(map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            // End Node
            MapNode endNode = map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
            while (nodeQueue.Count > 0)
            {
                Parallel.ForEach(nodeQueue, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (i) =>
                {
                    MapNode currNode;
                    nodeQueue.TryDequeue(out currNode);

                    lock (currNode.NodeLock)
                    {
                        currNode.Path += ($":{currNode.Position.X.ToString()},{currNode.Position.Y.ToString()}");
                        currNode.NodeValue = 2;

                        if (currNode.Position == endNode.Position)
                        {
                            return;
                        }
                        else
                        {
                            if (currNode.NorthNode != null && currNode.NorthNode.NodeValue == 0)
                            {
                                currNode.NorthNode.Path = currNode.Path;
                                currNode.NorthNode.NodeValue = 1;

                                nodeQueue.Enqueue(currNode.NorthNode);
                            }
                            if (currNode.EastNode != null && currNode.EastNode.NodeValue == 0)
                            {
                                currNode.EastNode.Path = currNode.Path;
                                currNode.EastNode.NodeValue = 1;

                                nodeQueue.Enqueue(currNode.EastNode);
                            }
                            if (currNode.SouthNode != null && currNode.SouthNode.NodeValue == 0)
                            {
                                currNode.SouthNode.Path = currNode.Path;
                                currNode.SouthNode.NodeValue = 1;

                                nodeQueue.Enqueue(currNode.SouthNode);
                            }
                            if (currNode.WestNode != null && currNode.WestNode.NodeValue == 0)
                            {
                                currNode.WestNode.Path = currNode.Path;
                                currNode.WestNode.NodeValue = 1;

                                nodeQueue.Enqueue(currNode.WestNode);
                            }
                        }

                        // Free resources
                        currNode.Path = null;
                    }
                });
            }

            return;
        }

        public static void DepthFirst(Map map)
        {
            //   let S be a stack
            //   S.push(v)
            //   while S is not empty
            //      v = S.pop()
            //      if v is not labeled as discovered:
            //          label v as discovered
            //          for all edges from v to w in G.adjacentEdges(v) do
            //              S.push(w)

            Stack<MapNode> nodeStack = new Stack<MapNode>();

            // Start node
            nodeStack.Push(map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            // End node
            MapNode endNode = map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            MapNode currNode;
            while (nodeStack.Count > 0)
            {
                currNode = nodeStack.Pop();
                currNode.NodeValue = 2;

                if (currNode.Position == endNode.Position)
                {
                    return;
                }
                else
                {
                    if (currNode.NorthNode != null && currNode.NorthNode.NodeValue == 0)
                    {
                        currNode.NorthNode.NodeValue = 1;
                        nodeStack.Push(currNode.NorthNode);
                    }
                    if (currNode.EastNode != null && currNode.EastNode.NodeValue == 0)
                    {
                        currNode.EastNode.NodeValue = 1;
                        nodeStack.Push(currNode.EastNode);
                    }
                    if (currNode.SouthNode != null && currNode.SouthNode.NodeValue == 0)
                    {
                        currNode.SouthNode.NodeValue = 1;
                        nodeStack.Push(currNode.SouthNode);
                    }
                    if (currNode.WestNode != null && currNode.WestNode.NodeValue == 0)
                    {
                        currNode.WestNode.NodeValue = 1;
                        nodeStack.Push(currNode.WestNode);
                    }
                }
            }

            return;
        }
    }
}
