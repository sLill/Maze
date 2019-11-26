using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        protected virtual bool RemoveExcursions()
        {
            bool HasExcursions = true;

            MapNode EndNode = Map.Nodes.Where(x => x.Value.IsEndNode).FirstOrDefault().Value;
            List<string> PathFull = string.Join(string.Empty, EndNode.GetPathSegments()).Split(':').ToList();

            for (int i = 0; i < Map.ImageColors.Length; i++)
            {
                Color[] colorRow = Map.ImageColors[i];
                for (int j = 0; j < colorRow.Length; j++)
                {
                    if (!PathFull.Contains($"{i},{j}"))
                    {
                        Map.ImageColors[i][j] = Color.FromArgb(255, 255, 255);
                    }
                    else
                    {
                        Map.ImageColors[i][j] = Color.FromArgb(0, 0, 0);
                    }
                }
            }

            Map.InitializeNodes();

            return HasExcursions;
        }

        protected virtual bool Search() { return true; }

        public virtual bool Solve()
        {
            bool Solved = false;
            while (!Solved)
            {
                this.Search();

                if (RemoveExcursions())
                {
                    Solved = true;
                }
            }

            return true;
        }
        #endregion Methods..
    }
}
