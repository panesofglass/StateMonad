using System;

namespace StateMonad.Base
{
    public static class Extensions
    {
        public static void Show<T>(this T thing, int level)
        {
            Console.Write("{0}", thing.ToString());
        }
    }
}