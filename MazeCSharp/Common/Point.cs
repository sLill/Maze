namespace Common
{
    public class Point
    {
        #region Member Variables..
        #endregion Member Variables..

        #region Properties..
        public int X { get; set; }

        public int Y { get; set; }
        #endregion Properties..

        #region Constructors..
        #endregion Constructors..

        #region Methods..
        public override string ToString()
        {
            return $"{X},{Y}";
        }
        #endregion Methods..
    }
}
