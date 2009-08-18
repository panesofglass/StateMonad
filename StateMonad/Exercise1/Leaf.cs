using System;

namespace StateMonad.Exercise1
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
            global::StateMonad.Exercise1.Extensions.Show(Contents, level);
            Console.WriteLine();
        }
    }
}