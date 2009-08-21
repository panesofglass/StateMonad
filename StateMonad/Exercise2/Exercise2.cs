using System;
using StateMonad.Base;
using StateMonad.Exercise1;

namespace StateMonad.Exercise2
{
    public static class Exercise2<TState, TContents>
    {
        private static Func<StateMonad<TState, TState>> _leftBounder;
        private static Func<StateMonad<TState, TState>> _rightBounder;

        public static void Run(Tree<TContents> tree,
                               TState seed,
                               Func<StateMonad<TState, TState>> leftBounder, 
                               Func<StateMonad<TState, TState>> rightBounder)
        {
            _leftBounder = leftBounder;
            _rightBounder = rightBounder;

            Console.WriteLine();
            Console.WriteLine("Exercise 2: Tree within bounded rects:");
            var rectTree = Bound(tree, seed);
            rectTree.Show(2);
            Console.WriteLine();
        }

        private static Tree<Tuple<TState, TContents>> Bound(Tree<TContents> tree, TState seed)
        {
            return MakeMonad(tree, _leftBounder).StateMaker(seed).Second;
        }

        private static StateMonad<TState, Tree<Tuple<TState, TContents>>> MakeMonad(
            Tree<TContents> t,
            Func<StateMonad<TState, TState>> bounder)
        {
            if (t is Leaf<TContents>)
            {
                var lf = (t as Leaf<TContents>);

                return bounder()
                    .Bind(n => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                        .Return(new Leaf<Tuple<TState, TContents>>(Tuple.Create(n, lf.Contents))));
            }

            if (t is Branch<TContents>)
            {
                var br = (t as Branch<TContents>);
                var oldleft = br.Left;
                var oldright = br.Right;

                return MakeMonad(oldleft, _leftBounder)
                    .Bind(newleft => MakeMonad(oldright, _rightBounder)
                        .Bind(newright => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                            .Return(new Branch<Tuple<TState, TContents>>(newleft, newright))));
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }
    }
}
