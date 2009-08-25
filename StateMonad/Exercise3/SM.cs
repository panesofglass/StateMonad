using System;
using StateMonad.Base;

namespace StateMonad.Exercise3
{
    public class SM<TState, TContents> : M<TContents>
    {
        public readonly Func<TState, Tuple<TState, TContents>> StateMaker;

        public SM() { }

        public SM(Func<TState, Tuple<TState, TContents>> stateMaker)
        {
            StateMaker = stateMaker;
        }

        public override M<TContents> Return(TContents contents)
        {
            return new SM<TState, TContents>(state => Tuple.Create(state, contents));
        }

        public override M<TContentsB> Bind<TContentsB>(Func<TContents, M<TContentsB>> inputMaker)
        {
            return new SM<TState, TContentsB>(state0 =>
            {
                var lcp1 = StateMaker(state0);
                var state1 = lcp1.First;
                var contents1 = lcp1.Second;

                return ((SM<TState, TContentsB>)inputMaker(contents1)).StateMaker(state1);
            });
        }
    }
}
