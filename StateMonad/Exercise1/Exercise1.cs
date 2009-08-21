using System;
using StateMonad.Base;

namespace StateMonad.Exercise1
{
    public static class Exercise1<TState, TContents>
    {
        private static Func<StateMonad<TState, TState>> _incrementer;

        public static void Run(Tree<TContents> tree, TState seed, Func<StateMonad<TState, TState>> incrementer)
        {
            _incrementer = incrementer;

            Console.WriteLine();
            Console.WriteLine("Exercise 1: Monadically Labeled Tree:");
            var t3 = MLabel(tree, seed);
            t3.Show(2);
            Console.WriteLine();
        }

        private static Tree<Tuple<TState, TContents>> MLabel(Tree<TContents> tree, TState seed)
        {
            return MakeMonad(tree, _incrementer).StateMaker(seed).Second;
        }

        private static StateMonad<TState, Tree<Tuple<TState, TContents>>> MakeMonad(
            Tree<TContents> t,
            Func<StateMonad<TState, TState>> incrementer)
        {
            if (t is Leaf<TContents>)
            {
                var lf = (t as Leaf<TContents>);

                return incrementer()
                    .Bind(n => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                        .Return(new Leaf<Tuple<TState, TContents>>(Tuple.Create(n, lf.Contents))));
            }

            if (t is Branch<TContents>)
            {
                var br = (t as Branch<TContents>);
                var oldleft = br.Left;
                var oldright = br.Right;

                return MakeMonad(oldleft, incrementer)
                    .Bind(newleft => MakeMonad(oldright, incrementer)
                        .Bind(newright => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                            .Return(new Branch<Tuple<TState, TContents>>(newleft, newright))));
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }
    }
}