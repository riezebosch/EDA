namespace Basket.EventHubs
{
    public interface ITestHookup<out T>: IHookup, IAssert<T>
    {
    }
}