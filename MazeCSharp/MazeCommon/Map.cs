using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeCommon
{
    public class Map
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public ConcurrentDictionary<string, MapNode> Nodes { get; set; }
        #endregion Properties..

        #region Constructors..
        public Map(int width, int height)
        {
            Nodes = new ConcurrentDictionary<string, MapNode>();
            Parallel.For(0, width, (i) =>
            {
                for (int j = 0; j < height; j++)
                {
                    string position = $"{i},{j}";
                    Nodes[position] = new MapNode() { Position = new Point() { X = i, Y = j } };
                }
            });
        }
        #endregion Constructors..

        #region Methods..
        #endregion Methods..
    }
}
