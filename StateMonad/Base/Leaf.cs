using System;

namespace StateMonad.Base
{
    public class Leaf<TContents> : Tree<TContents>
    {
        public readonly TContents Contents;

        public Leaf(TContents contents)
        {
            Contents = contents;
        }

        public override void Show(int level)
        {
            Console.Write(new String(' ', level * Settings.Indentation));
            Console.Write("Leaf: ");
            Contents.Show(level);
            Console.WriteLine();
        }
    }
}