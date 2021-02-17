using System;

namespace EDA.TestAdapters
{
    public interface IAssert<out T>
    {
        void Assert(Action<T> assert, TimeSpan timeout);
        void Assert(Action<T> assert);
    }
}