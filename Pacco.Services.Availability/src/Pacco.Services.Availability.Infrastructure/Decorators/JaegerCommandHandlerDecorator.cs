using System;
using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using MicroPack.MicroPack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenTracing;
using OpenTracing.Tag;

namespace Pacco.Services.Availability.Infrastructure.Decorators
{
    public class JaegerCommandHandlerDecorator<T>: ICommandHandler<T> where T : class , ICommand
    {
        private readonly ICommandHandler<T> _handler;
        private readonly ITracer _tracer;

        public JaegerCommandHandlerDecorator(ICommandHandler<T> handler, ITracer tracer)
        {
            _handler = handler;
            _tracer = tracer;
        }
        public async Task HandleAsync(T command)
        {
            var commandName = command.GetType().Name.Underscore();
            using var scope = BuidScope(commandName);
            var span = scope.Span;
            try
            {
                span.Log($"handeling a message : '{commandName}'");
                await _handler.HandleAsync(command);
                span.Log($"handeled a message : '{commandName}'");
            }
            catch (Exception e)
            {
                span.Log(e.Message);
                span.SetTag(Tags.Error, true);
                throw;
            }
        }

        private IScope BuidScope(string commandName)
        {
            var scope = _tracer
                .BuildSpan($"handeling_{commandName}")
                .WithTag($"message-type", commandName);
            if (_tracer.ActiveSpan is {})
            {
                scope.AddReference(References.ChildOf, _tracer.ActiveSpan.Context);
            }

            return scope.StartActive(true);
        }
    }
}