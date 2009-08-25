using System;

namespace StateMonad.Exercise3
{
    public abstract class M<TContents>
    {
        public abstract M<TContents> Return(TContents contents);

        public abstract M<TContentsB> Bind<TContentsB>(Func<TContents, M<TContentsB>> inputMaker);
    }
}
