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
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion Constructors..

        #region Methods..
        public static Point FromString(string pointString)
        {
            return new Point(Convert.ToInt32(pointString.Split(',')[0]), Convert.ToInt32(pointString.Split(',')[1]));
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static double operator -(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1?.X == p2?.X && p1?.Y == p2?.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1?.X == p2?.X && p1?.Y == p2?.Y);
        }
        #endregion Methods..
    }
}
