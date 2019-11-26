using System;

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
        public static Point FromString(string pointString)
        {
            return new Point()
            {
                X = Convert.ToInt32(pointString.Split(',')[0]),
                Y = Convert.ToInt32(pointString.Split(',')[1])
            };
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
        #endregion Methods..
    }
}
