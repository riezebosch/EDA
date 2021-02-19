using System;
using System.Threading.Tasks;

namespace EDA.EventHubs
{
    public interface IProcessor : IAsyncDisposable
    {
        Task Start();
    }
}