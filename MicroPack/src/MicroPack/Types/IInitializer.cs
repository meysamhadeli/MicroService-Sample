using System.Threading.Tasks;

namespace MicroPack.MicroPack.Types
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}