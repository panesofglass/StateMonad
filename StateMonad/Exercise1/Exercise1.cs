using System;
using StateMonad.Base;

namespace StateMonad.Exercise1
{
    public static class Exercise1<TState, TContents>
    {
        public static void Run(Tree<TContents> tree, TState seed, Func<StateMonad<TState, TState>> incrementer)
        {
            var runner = new Runner<TState, TContents>(incrementer);

            Console.WriteLine();
            Console.WriteLine("Exercise 1: Monadically Labeled Tree:");
            var t3 = runner.MLabel(tree, seed);
            t3.Show(2);
            Console.WriteLine();
        }
    }
}