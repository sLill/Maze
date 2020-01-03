using Common;
using System.Collections.Generic;
using System.Linq;

namespace Implementation
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
        protected override bool Search()
        {
            ////   let S be a stack
            ////   S.push(v)
            ////   while S is not empty
            ////      v = S.pop()
            ////      if v is not labeled as discovered:
            ////          label v as discovered
            ////          for all edges from v to w in G.adjacentEdges(v) do
            ////              S.push(w)

            //Stack<MapNode> NodeStack = new Stack<MapNode>();

            //// Start node
            //NodeStack.Push(Map.Nodes.Where(x => x.Value.IsStartNode).FirstOrDefault().Value);

            //// End node
            //MapNode EndNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;

            //MapNode CurrentNode;
            //while (NodeStack.Count > 0)
            //{
            //    CurrentNode = NodeStack.Pop();
            //    CurrentNode.Path += ($":{CurrentNode.Position.X.ToString()},{CurrentNode.Position.Y.ToString()}");
            //    CurrentNode.NodeValue = 2;

            //    if (CurrentNode.Position == EndNode.Position)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        if (CurrentNode.NorthNode != null && CurrentNode.NorthNode.NodeValue == 0)
            //        {
            //            CurrentNode.NorthNode.Path = CurrentNode.Path;
            //            CurrentNode.NorthNode.NodeValue = 1;
            //            NodeStack.Push(CurrentNode.NorthNode);
            //        }
            //        if (CurrentNode.EastNode != null && CurrentNode.EastNode.NodeValue == 0)
            //        {
            //            CurrentNode.EastNode.Path = CurrentNode.Path;
            //            CurrentNode.EastNode.NodeValue = 1;
            //            NodeStack.Push(CurrentNode.EastNode);
            //        }
            //        if (CurrentNode.SouthNode != null && CurrentNode.SouthNode.NodeValue == 0)
            //        {
            //            CurrentNode.SouthNode.Path = CurrentNode.Path;
            //            CurrentNode.SouthNode.NodeValue = 1;
            //            NodeStack.Push(CurrentNode.SouthNode);
            //        }
            //        if (CurrentNode.WestNode != null && CurrentNode.WestNode.NodeValue == 0)
            //        {
            //            CurrentNode.WestNode.Path = CurrentNode.Path;
            //            CurrentNode.WestNode.NodeValue = 1;
            //            NodeStack.Push(CurrentNode.WestNode);
            //        }
            //    }

            //    CurrentNode.Path = null;
            //}

            return base.Search();
        }
        #endregion Methods..
    }
}
