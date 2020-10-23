using System.Threading.Tasks;

namespace MicroPack.Types
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}