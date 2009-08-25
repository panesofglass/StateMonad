using System;
using StateMonad.Base;

namespace StateMonad.Exercise5
{
    public delegate Tuple<TState, TContents> State<TState, TContents>(TState state);

    public static class StateBuilder
    {
        /// <summary>
        /// The map operator. This is the default m.Let function in the StateBuilder of Exercise1_2.fs.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TContentsA">The original content type.</typeparam>
        /// <typeparam name="TContentsB">The transformed content type.</typeparam>
        /// <param name="monad">The monad.</param>
        /// <param name="selector">The map function.</param>
        /// <returns>A new monad monad.</returns>
        public static State<TState, TContentsB> Select<TState, TContentsA, TContentsB>(
            this State<TState, TContentsA> monad,
            Func<TContentsA, TContentsB> selector)
        {
            return state => Tuple.Create(state, selector(monad(state).Second));
        }

        /// <summary>
        /// The bind operator. This is nearly an exact duplication of the m.Bind operator in StateBuilder of Exercise1_2.fs.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TContentsA">The initial type of the content.</typeparam>
        /// <typeparam name="TContentsB">The interim type of the content.</typeparam>
        /// <typeparam name="TContentsC">The final type of the content.</typeparam>
        /// <param name="monad">The monad.</param>
        /// <param name="selector">The map function.</param>
        /// <param name="projector">The flatten map function.</param>
        /// <returns>A new monad monad.</returns>
        public static State<TState, TContentsC> SelectMany<TState, TContentsA, TContentsB, TContentsC>(
            this State<TState, TContentsA> monad,
            Func<TContentsA, State<TState, TContentsB>> selector,
            Func<TContentsA, TContentsB, TContentsC> projector)
        {
            return state =>
            {
                var first = monad(state);
                var second = selector(first.Second)(first.First);
                var content = projector(first.Second, second.Second);
                return Tuple.Create(second.First, content);
            };
        }

        /// <summary>
        /// Gets the state. This is the same as the getState function in Exercise1_2.fs.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <returns></returns>
        public static State<TState, TState> GetState<TState>()
        {
            return state => Tuple.Create(state, state);
        }

        /// <summary>
        /// Sets the state. This is the same as the setState function in Exercise1_2.fs.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TContents">The type of the contents.</typeparam>
        /// <param name="newState">The new state.</param>
        /// <returns></returns>
        public static State<TState, TContents> SetState<TState, TContents>(TState newState)
        {
            return state => Tuple.Create(newState, default(TContents));
        }
    }
}