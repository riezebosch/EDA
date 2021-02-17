using System;

namespace Basket.TestAdapters
{
    public interface IAssert<out T>
    {
        void Assert(Action<T> assert, TimeSpan timeout);
        void Assert(Action<T> assert);
    }
}