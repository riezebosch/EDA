using EDA.TestAdapters;

namespace EDA.EventHubs
{
    public interface ITestHookup<out T>: IHookup, IAssert<T>
    {
    }
}