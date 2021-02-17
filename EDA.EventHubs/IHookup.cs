using System;
using System.Threading.Tasks;

namespace EDA.EventHubs
{
    public interface IHookup : IAsyncDisposable
    {
        Task Start();
    }
}