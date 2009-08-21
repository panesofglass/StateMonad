using System;
using StateMonad.Base;

namespace StateMonad.Exercise1
{
    public class StateMonad<TState, TContents>
    {
        public readonly Func<TState, Tuple<TState, TContents>> StateMaker;

        public StateMonad(Func<TState, Tuple<TState, TContents>> stateMaker)
        {
            StateMaker = stateMaker;
        }

        public static StateMonad<TState, TContents> Return(TContents contents)
        {
            return new StateMonad<TState, TContents>(state => Tuple.Create(state, contents));
        }

        public static StateMonad<TState, TContentsB> Bind<TContentsB>(
            StateMonad<TState, TContents> inputMonad,
            Func<TContents, StateMonad<TState, TContentsB>> inputMaker)
        {
            return new StateMonad<TState, TContentsB>(state0 =>
            {
                var lcp1 = inputMonad.StateMaker(state0);
                var state1 = lcp1.First;
                var contents1 = lcp1.Second;

                return inputMaker(contents1).StateMaker(state1);
            });
        }
    }

    public static class StateMonad
    {
        public static StateMonad<TState, TContents2> Bind<TState, TContents, TContents2>(
            this StateMonad<TState, TContents> inputMonad,
            Func<TContents, StateMonad<TState, TContents2>> inputMaker)
        {
            return StateMonad<TState, TContents>.Bind(inputMonad, inputMaker);
        }
    }
}