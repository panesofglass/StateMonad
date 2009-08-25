using System;
using StateMonad.Base;
using StateMonad.Exercise2;

namespace StateMonad.Exercise5
{
    public static class Exercise5
    {
        /// <summary>
        /// Runs the specified tree.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="seed">The seed.</param>
        /// <remarks>
        /// For this exercise, it's helpful to look at the monad builder syntax in F#.
        /// In my case, that was Exercise1_2.fs. I've noted the use and similarities
        /// of that implementation where it was used here.
        /// </remarks>
        public static void Run(Tree<string> tree, Tuple<double, Rect> seed)
        {
            State<Tuple<double, Rect>, Tuple<double, Rect>> leftUpdater = state =>
                {
                    var depth = state.First;
                    var rect = state.Second;
                    var newDepth = depth + 1.0;
                    var multiplier = 2.0 * newDepth;
                    var nextRect = new Rect(rect.Height, rect.Width / multiplier, rect.Top, rect.Left + rect.Width / multiplier);
                    var currRect = new Rect(rect.Height, rect.Width / multiplier, rect.Top, rect.Left);
                    return Tuple.Create(Tuple.Create(newDepth, nextRect), Tuple.Create(newDepth, currRect));
                };

            State<Tuple<double, Rect>, Tuple<double, Rect>> rightUpdater = state =>
                {
                    var depth = state.First;
                    var rect = state.Second;
                    var newDepth = depth - 1.0;
                    var nextRect = new Rect(rect.Height, rect.Width * 2, rect.Top, rect.Left + rect.Width);
                    return Tuple.Create(Tuple.Create(newDepth, nextRect), state);
                };

            var runner = new Runner<Tuple<double, Rect>, string>(leftUpdater, rightUpdater);

            Console.WriteLine();
            Console.WriteLine("Exercise 5: Tree within bounded rects using LINQ monad:");
            var rectTree = runner.MLabel(tree, seed);
            rectTree.Show(2);
            Console.WriteLine();
        }
    }
}
