using System.Threading.Tasks;

namespace EDA.Ports
{
    public interface ISubscribe<in T>
    {
        Task Handle(T body);
    }
}