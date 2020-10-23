using MicroPack.Types;

namespace MicroPack.MicroPack.Types
{
    public interface IStartupInitializer : IInitializer
    {
        void AddInitializer(IInitializer initializer);
    }
}