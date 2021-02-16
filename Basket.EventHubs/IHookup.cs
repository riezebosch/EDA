using System;
using System.Threading.Tasks;

namespace Basket.EventHubs
{
    public interface IHookup : IAsyncDisposable
    {
        Task Start();
    }
}