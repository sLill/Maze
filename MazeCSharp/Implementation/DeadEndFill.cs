using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation
{
    public class DeadEndFill : TraversalType
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        #endregion Properties..

        #region Constructors..
        public DeadEndFill(Map map)
            : base(map) { }
        #endregion Constructors..

        #region Methods..
        private void RemoveDeadEnd(ConcurrentDictionary<int, ConcurrentDictionary<int, MapNode>> mapNodes, Point nodePosition)
        {
            if (nodePosition != null)
            {
                if (mapNodes.ContainsKey(nodePosition.X) && mapNodes[nodePosition.X].ContainsKey(nodePosition.Y)
                    && mapNodes[nodePosition.X][nodePosition.Y].ConnectedNodes < 3)
                {
                    mapNodes[nodePosition.X].TryRemove(nodePosition.Y, out MapNode Node);

                    RemoveDeadEnd(mapNodes, Node.NorthNode);
                    RemoveDeadEnd(mapNodes, Node.EastNode);
                    RemoveDeadEnd(mapNodes, Node.SouthNode);
                    RemoveDeadEnd(mapNodes, Node.WestNode);
                }
            }
        }

        protected override bool Search()
        {
            // Dead-end filling is an algorithm that finds its' solution through filling/or removing all dead-ends leaving only the solution remaining untouched
            int NodeCount = Map.Nodes.Sum(x => x.Value.Count);

            while (true)
            {
                Parallel.ForEach(Map.Nodes, row =>
                {
                    var DeadEndNodes = new Stack<MapNode>(Map.Nodes[row.Key].Where(x => x.Value.ConnectedNodes == 1
                        && x.Value.Position.ToString() != Map.StartNodePosition.ToString()
                        && x.Value.Position.ToString() != Map.EndNodePosition.ToString()).Select(x => x.Value));

                    while (DeadEndNodes.Count() > 0)
                    {
                        RemoveDeadEnd(Map.Nodes, DeadEndNodes.Pop().Position);
                    }
                });


                //Map RevisedMap = new Map(FullPath);
                //RevisedMap.InitializeAsync().Wait();

                MapNode EndNode = this.Map.Nodes[Map.EndNodePosition.X][Map.EndNodePosition.Y];
                //Parallel.ForEach(Map.Nodes, row =>
                foreach (var row in Map.Nodes)
                {
                    foreach (var nodePosition in row.Value.Values.Select(x => x.Position))
                    {
                        EndNode.AppendPointToPath(nodePosition);
                    }
                };
            }

            return base.Search();
        }
        #endregion Methods..
    }
}
