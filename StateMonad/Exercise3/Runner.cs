using System;
using StateMonad.Base;

namespace StateMonad.Exercise3
{
    public class Runner<TState, TContents>
    {
        private static Func<SM<TState, TState>> _leftUpdater;
        private static Func<SM<TState, TState>> _rightUpdater;

        public Runner(Func<SM<TState, TState>> leftUpdater, Func<SM<TState, TState>> rightUpdater)
        {
            _leftUpdater = leftUpdater;
            _rightUpdater = rightUpdater;
        }

        public Tree<Tuple<TState, TContents>> MLabel(Tree<TContents> tree, TState seed)
        {
            return ((SM<TState, Tree<Tuple<TState, TContents>>>)MakeMonad(tree, _leftUpdater)).StateMaker(seed).Second;
        }

        private static M<Tree<Tuple<TState, TContents>>> MakeMonad(
            Tree<TContents> t,
            Func<SM<TState, TState>> updater)
        {
            if (t is Leaf<TContents>)
            {
                var lf = (t as Leaf<TContents>);

                return updater()
                    .Bind(n => new SM<TState, Tree<Tuple<TState, TContents>>>()
                        .Return(new Leaf<Tuple<TState, TContents>>(Tuple.Create(n, lf.Contents))));
            }

            if (t is Branch<TContents>)
            {
                var br = (t as Branch<TContents>);
                var oldleft = br.Left;
                var oldright = br.Right;

                return MakeMonad(oldleft, _leftUpdater)
                    .Bind(newleft => MakeMonad(oldright, _rightUpdater)
                        .Bind(newright => new SM<TState, Tree<Tuple<TState, TContents>>>()
                            .Return(new Branch<Tuple<TState, TContents>>(newleft, newright))));
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }
    }
}