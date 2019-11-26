using Common;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <summary>
        /// Returns true if the solution path contains any nodes with more than two connections
        /// </summary>
        /// <returns></returns>
        protected virtual bool PathHasExcursions()
        {
            bool HasExcursions = false;

            MapNode EndNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
            var PathSegments = EndNode.GetPathSegments();

            foreach (var segment in PathSegments)
            {
                List<string> Positions = segment?.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                HasExcursions = HasExcursions || Positions.ToList()
                    .Where(x => Map.Nodes[x].ConnectedNodes.Where(y => Positions.Contains(y.Position.ToString())).Count() > 2)
                    .Any();
            }

            return HasExcursions;
        }

        protected virtual bool Search() { return true; }

        public virtual bool Solve()
        {
            bool Solved = false;
            while (!Solved)
            {
                this.Search();

                if (PathHasExcursions())
                {
                    Map.RefreshNodeCollection();
                }
                else
                {
                    Solved = true;
                }
            }

            return true;
        }
        #endregion Methods..
    }
}
