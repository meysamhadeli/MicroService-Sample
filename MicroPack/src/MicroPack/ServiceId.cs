using System;

namespace MicroPack.MicroPack
{
    internal class ServiceId : IServiceId
    {
        public string Id { get; } = $"{Guid.NewGuid():N}";
    }
}