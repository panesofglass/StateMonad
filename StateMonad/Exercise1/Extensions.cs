using System;

namespace StateMonad.Exercise1
{
    public static class Extensions
    {
        public static void Show<a>(this a thing, int level)
        {
            Console.Write("{0}", thing.ToString());
        }
    }
}