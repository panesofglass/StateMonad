using System;
using StateMonad.Base;
using StateMonad.Exercise1;

namespace StateMonad.Exercise2
{
    public class Runner<TState, TContents>
    {
        private static Func<StateMonad<TState, TState>> _leftUpdater;
        private static Func<StateMonad<TState, TState>> _rightUpdater;

        public Runner(Func<StateMonad<TState, TState>> leftUpdater, Func<StateMonad<TState, TState>> rightUpdater)
        {
            _leftUpdater = leftUpdater;
            _rightUpdater = rightUpdater;
        }

        public Tree<Tuple<TState, TContents>> MLabel(Tree<TContents> tree, TState seed)
        {
            return MakeMonad(tree, _leftUpdater).StateMaker(seed).Second;
        }

        private static StateMonad<TState, Tree<Tuple<TState, TContents>>> MakeMonad(Tree<TContents> t, Func<StateMonad<TState, TState>> updater)
        {
            if (t is Leaf<TContents>)
            {
                var lf = (t as Leaf<TContents>);

                return updater()
                    .Bind(n => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                        .Return(new Leaf<Tuple<TState, TContents>>(Tuple.Create(n, lf.Contents))));
            }

            if (t is Branch<TContents>)
            {
                var br = (t as Branch<TContents>);
                var oldleft = br.Left;
                var oldright = br.Right;

                return MakeMonad(oldleft, _leftUpdater)
                    .Bind(newleft => MakeMonad(oldright, _rightUpdater)
                        .Bind(newright => StateMonad<TState, Tree<Tuple<TState, TContents>>>
                            .Return(new Branch<Tuple<TState, TContents>>(newleft, newright))));
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }
    }
}