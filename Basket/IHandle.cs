using System.Threading.Tasks;

namespace Basket
{
    public interface IHandle<in T>
    {
        Task Handle(T body);
    }
}