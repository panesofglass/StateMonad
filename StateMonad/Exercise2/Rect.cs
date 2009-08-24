namespace StateMonad.Exercise2
{
    public class Rect
    {
        public readonly double Height;
        public readonly double Width;
        public readonly double Top;
        public readonly double Left;

        public Rect(double height, double width, double top, double left)
        {
            Height = height;
            Width = width;
            Top = top;
            Left = left;
        }

        public override string ToString()
        {
            return string.Format("Height: {0}, Width: {1}, Top: {2}, Left: {3}", Height, Width, Top, Left);
        }
    }
}