namespace Common
{
    public class MapNode
    {
        #region Properties..
        public MapNode NorthNode { get; set; }

        public MapNode EastNode { get; set; }

        public MapNode SouthNode { get; set; }

        public MapNode WestNode { get; set; }

        public Point Position { get; set; }

        public string Path { get; set; }

        // -1 = Wall, 0 = Path (unvisited), 1 = Path (visited), 2 = Path (visited and examined)
        public int NodeValue { get; set; }

        public object NodeLock { get; set; }

        public bool IsStartNode { get; set; }

        public bool IsEndNode { get; set; }
        #endregion Properties..

        #region Constructors..
        public MapNode()
        {
            Path = string.Empty;
            NodeLock = new object();
        }
        #endregion Constructors..
    }
}
