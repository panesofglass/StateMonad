using System;

namespace StateMonad.Exercise1
{
    public static class Exercise1
    {
        private static StateMonad<TState, Tree<Tuple<TState, TContents>>> MakeMonad<TState, TContents>(
            Tree<TContents> t,
            Func<StateMonad<TState, TState>> contentMaker)
        {
            if (t is Leaf<TContents>)
            {
                var lf = (t as Leaf<TContents>);

                return contentMaker().Bind(
                    n => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                        .Return(new Leaf<Tuple<TState, TContents>>(Tuple.Create(n, lf.Contents))));
            }

            if (t is Branch<TContents>)
            {
                var br = (t as Branch<TContents>);
                var oldleft = br.Left;
                var oldright = br.Right;

                return MakeMonad<TState, TContents>(oldleft, contentMaker)
                    .Bind(newleft => MakeMonad<TState, TContents>(oldright, contentMaker)
                        .Bind(newright => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                            .Return(new Branch<Tuple<TState, TContents>>(newleft, newright))));
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }

        private static Tree<Tuple<TState, TContents>> MLabel<TState, TContents>(
            Tree<TContents> t,
            Func<StateMonad<TState, TState>> contentMaker)
        {
            return MakeMonad<TState, TContents>(t, contentMaker).StateMaker(default(TState)).Second;
        }

        public static void Run(Tree<string> tree)
        {
            Console.WriteLine();
            Console.WriteLine("Exercise 1: Monadically Labeled Tree:");
            Func<StateMonad<int, int>> contentMaker = () => new StateMonad<int, int>(n => Tuple.Create(n + 1, n));
            var t3 = MLabel<int, string>(tree, contentMaker);
            t3.Show(2);
            Console.WriteLine();
        }
    }
}