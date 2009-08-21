namespace StateMonad.Exercise2
{
    public class Rect
    {
        public readonly int Height;
        public readonly int Width;
        public readonly int Top;
        public readonly int Left;

        public Rect(int height, int width, int top, int left)
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