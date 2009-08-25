using System;
using StateMonad.Base;

namespace StateMonad.Exercise5
{
    public class Runner<TState, TContents>
    {
        private static State<TState, TState> _leftUpdater;
        private static State<TState, TState> _rightUpdater;

        public Runner(State<TState, TState> leftUpdater, State<TState, TState> rightUpdater)
        {
            _leftUpdater = leftUpdater;
            _rightUpdater = rightUpdater;
        }

        /// <summary>
        /// This is almost exactly like the labelTree function in Exercise1_2.fs.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        public Tree<Tuple<TState, TContents>> MLabel(Tree<TContents> tree, TState seed)
        {
            return MakeMonad(tree, _leftUpdater)(seed).Second;
        }

        /// <summary>
        /// This is almost exactly like the labelTree function in Exercise1_2.fs.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="updater">The updater.</param>
        /// <returns></returns>
        private static State<TState, Tree<Tuple<TState, TContents>>> MakeMonad(Tree<TContents> tree, State<TState, TState> updater)
        {
            if (tree is Leaf<TContents>)
            {
                var leaf = tree as Leaf<TContents>;
                return from s in StateBuilder.GetState<TState>()
                       let updatedStates = updater(s)
                       from f in StateBuilder.SetState<TState, TContents>(updatedStates.First)
                       select (Tree<Tuple<TState, TContents>>)new Leaf<Tuple<TState, TContents>>(Tuple.Create(updatedStates.Second, leaf.Contents));
            }

            if (tree is Branch<TContents>)
            {
                var branch = tree as Branch<TContents>;
                return from left in MakeMonad(branch.Left, _leftUpdater)
                       from right in MakeMonad(branch.Right, _rightUpdater)
                       select (Tree<Tuple<TState, TContents>>)new Branch<Tuple<TState, TContents>>(left, right);
            }

            throw new Exception("MakeMonad/MLabel: impossible Tree subtype");
        }
    }
}
