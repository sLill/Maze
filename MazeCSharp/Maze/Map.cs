using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class Map
    {
        public ConcurrentDictionary<string, MapNode> Nodes { get; set; }

        public Map(int width, int height)
        {
            Nodes = new ConcurrentDictionary<string, MapNode>();
            Parallel.For(0, width, (i) => {
                for (int j = 0; j < height; j++)
                {
                    string position = $"{i},{j}";
                    Nodes[position] = new MapNode() { Position = new Point() { X = i, Y = j } };
                }
            });
        }
    }

    public class MapNode
    {
        public MapNode NorthNode { get; set; }

        public MapNode EastNode { get; set; }

        public MapNode SouthNode { get; set; }

        public MapNode WestNode { get; set; }

        public Point Position { get; set; }

        public string Path { get; set; }

        // -1 = Wall, 0 = Path (unvisited), 1 = Path (visited), 2 = Path (visited and examined)
        public int NodeValue { get; set; }

        public object NodeLock { get; set; }

        //public int ConnectedNodes { get; set; }

        //public bool IsJunctionNode { get; set; }

        public bool IsStartNode { get; set; }

        public bool IsEndNode { get; set; }

        public MapNode()
        {
            Path = string.Empty;
            NodeLock = new object();
            //ConnectedNodes = 0;
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {

        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
