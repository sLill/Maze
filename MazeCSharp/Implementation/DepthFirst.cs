using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            Stack<MapNode> NodeStack = new Stack<MapNode>();

            // Start Node
            NodeStack.Push(Map.Nodes[Map.StartNodePosition.X][Map.StartNodePosition.Y]);

            MapNode CurrentNode;
            while (NodeStack.Count > 0)
            {
                CurrentNode = NodeStack.Pop();
                CurrentNode.AppendPointToPath(CurrentNode.Position);
                CurrentNode.NodeValue = 2;

                // Push to preview buffer
                Map.PreviewPixelBuffer.Push(CurrentNode.Position);

                if (CurrentNode.Position == Map.EndNodePosition)
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

                    Stack<StringBuilder> PathCopies = new Stack<StringBuilder>();
                    for (int i = 1; i < NeighborPositions.Count; i++)
                    {
                        PathCopies.Push(new StringBuilder(CurrentNode.Path.ToString()));
                    }

                    foreach (Point neighborPosition in NeighborPositions)
                    {
                        if (neighborPosition != null && Map.Nodes.ContainsKey(neighborPosition.X) && Map.Nodes[neighborPosition.X].ContainsKey(neighborPosition.Y)
                            && Map.Nodes[neighborPosition.X][neighborPosition.Y].NodeValue == 0)
                        {
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].Path = PathCopies.Any() ? PathCopies.Pop() : CurrentNode.Path;
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].NodeValue = 1;
                            Map.Nodes[neighborPosition.X][neighborPosition.Y].MemoryMappedFileManager = CurrentNode.MemoryMappedFileManager;

                            NodeStack.Push(Map.Nodes[neighborPosition.X][neighborPosition.Y]);
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
