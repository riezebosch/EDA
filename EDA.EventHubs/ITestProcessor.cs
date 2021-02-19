using EDA.TestAdapters;

namespace EDA.EventHubs
{
    public interface ITestProcessor<out T>: IProcessor, IAssert<T>
    {
    }
}