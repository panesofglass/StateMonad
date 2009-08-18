using System;

namespace StateMonad.Exercise1
{
    public class Branch<TContents> : Tree<TContents>
    {
        public readonly Tree<TContents> Left;
        public readonly Tree<TContents> Right;

        public Branch(Tree<TContents> left, Tree<TContents> right)
        {
            Left = left;
            Right = right;
        }

        public override void Show(int level)
        {
            Console.Write(new String(' ', level * Settings.Indentation));
            Console.WriteLine("Branch:");
            Left.Show(level + 1);
            Right.Show(level + 1);
        }
    }
}