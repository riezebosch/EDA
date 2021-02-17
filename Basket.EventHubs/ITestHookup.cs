using Basket.TestAdapters;

namespace Basket.EventHubs
{
    public interface ITestHookup<out T>: IHookup, IAssert<T>
    {
    }
}