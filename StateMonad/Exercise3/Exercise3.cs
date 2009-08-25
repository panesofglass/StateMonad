using System;
using StateMonad.Base;
using StateMonad.Exercise2;

namespace StateMonad.Exercise3
{
    public static class Exercise3
    {
        public static void Run(Tree<string> tree, Tuple<double, Rect> seed)
        {
            Func<SM<Tuple<double, Rect>, Tuple<double, Rect>>> leftUpdater = () =>
            new SM<Tuple<double, Rect>, Tuple<double, Rect>>(m =>
            {
                var depth = m.First;
                var rect = m.Second;
                var newDepth = depth + 1.0;
                var multiplier = 2.0 * newDepth;
                var nextRect = new Rect(rect.Height, rect.Width / multiplier, rect.Top, rect.Left + rect.Width / multiplier);
                var currRect = new Rect(rect.Height, rect.Width / multiplier, rect.Top, rect.Left);
                return Tuple.Create(Tuple.Create(newDepth, nextRect), Tuple.Create(newDepth, currRect));
            });

            Func<SM<Tuple<double, Rect>, Tuple<double, Rect>>> rightUpdater = () =>
                new SM<Tuple<double, Rect>, Tuple<double, Rect>>(m =>
                {
                    var depth = m.First;
                    var rect = m.Second;
                    var newDepth = depth - 1.0;
                    var nextRect = new Rect(rect.Height, rect.Width * 2, rect.Top, rect.Left + rect.Width);
                    return Tuple.Create(Tuple.Create(newDepth, nextRect), m);
                });

            var runner = new Runner<Tuple<double, Rect>, string>(leftUpdater, rightUpdater);

            Console.WriteLine();
            Console.WriteLine("Exercise 2: Tree within bounded rects:");
            var rectTree = runner.MLabel(tree, seed);
            rectTree.Show(2);
            Console.WriteLine();
        }
    }
}
