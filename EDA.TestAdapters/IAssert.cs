using System;
using System.Threading.Tasks;

namespace EDA.TestAdapters
{
    public interface IAssert<out T>
    {
        Task Assert(Action<T> assert, TimeSpan timeout);
        Task Assert(Action<T> assert);
    }
}