using System;
using MicroPack.MicroPack.Types;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.MicroPack
{
    public interface IConveyBuilder
    {
        IServiceCollection Services { get; }
        bool TryRegister(string name);
        void AddBuildAction(Action<IServiceProvider> execute);
        void AddInitializer(IInitializer initializer);
        void AddInitializer<TInitializer>() where TInitializer : IInitializer;
        IServiceProvider Build();
    }
}