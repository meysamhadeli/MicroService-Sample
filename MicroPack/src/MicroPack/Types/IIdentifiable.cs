using System;

namespace MicroPack.MicroPack.Types
{
    public interface IIdentifiable : IIdentifiable<Guid>
    {
        
    }
    
    public interface IIdentifiable<out T>
    {
        T Id { get; }
    }
}