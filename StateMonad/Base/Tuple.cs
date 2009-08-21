using System;

namespace StateMonad.Base
{
    public class Tuple<TFirst, TSecond>
    {
        public readonly TFirst First;
        public readonly TSecond Second;

        public Tuple(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return String.Format("First: {0}, Second: {1}", First, Second);
        }
    }

    public static class Tuple
    {
        public static Tuple<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new Tuple<TFirst, TSecond>(first, second);
        }
    }
}