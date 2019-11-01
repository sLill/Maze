using MazeCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeTraversal
{
    public class DepthFirst : TraversalType
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public DepthFirst(Map map)
            : base(map) { }
        #endregion Constructors..

        #region Methods..
        protected override bool SolveSingleThreaded()
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
            nodeStack.Push(Map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            // End node
            MapNode endNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            MapNode currNode;
            while (nodeStack.Count > 0)
            {
                currNode = nodeStack.Pop();
                currNode.NodeValue = 2;

                if (currNode.Position == endNode.Position)
                {
                    return true;
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

            return base.SolveSingleThreaded();
        }
        #endregion Methods..
    }
}
