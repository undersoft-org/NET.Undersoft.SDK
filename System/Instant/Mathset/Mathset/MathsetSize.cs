namespace System.Instant.Mathset
{
    using System;

    [Serializable]
    public class MathsetSize
    {
        public static MathsetSize Scalar = new MathsetSize(1, 1);
        public int cols;
        public int rows;

        public MathsetSize(int i, int j)
        {
            rows = i;
            cols = j;
        }

        public override bool Equals(object o)
        {
            if (o is MathsetSize)
                return ((MathsetSize)o) == this;
            return false;
        }

        public override int GetHashCode()
        {
            return rows * cols;
        }

        public override string ToString()
        {
            return "" + rows + " " + cols;
        }

        public static bool operator !=(MathsetSize o1, MathsetSize o2)
        {
            return o1.rows != o2.rows || o1.cols != o2.cols;
        }

        public static bool operator ==(MathsetSize o1, MathsetSize o2)
        {
            return o1.rows == o2.rows && o1.cols == o2.cols;
        }
    }
}
