using System;
using StateMonad.Base;

namespace StateMonad.Exercise1
{
    public static class Exercise1
    {
        public static void Run(Tree<string> tree, int seed)
        {
            Func<StateMonad<int, int>> incrementer = () => new StateMonad<int, int>(n => Tuple.Create(n + 1, n));

            var runner = new Runner<int, string>(incrementer);

            Console.WriteLine();
            Console.WriteLine("Exercise 1: Monadically Labeled Tree:");
            var t3 = runner.MLabel(tree, seed);
            t3.Show(2);
            Console.WriteLine();
        }
    }
}