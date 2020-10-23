using System;
using System.Collections.Generic;
using System.Linq;
using MicroPack.CQRS.Commands;

namespace Pacco.Services.Availability.Application.Commands
{
    public class AddResource : ICommand
    {
        public Guid ResourceId { get; }
        public IEnumerable<string> Tags { get; }

        public AddResource(Guid resourceId, IEnumerable<string> tags)
            => (ResourceId, Tags) = (resourceId == Guid.Empty ? Guid.NewGuid() : resourceId,
                tags ?? Enumerable.Empty<string>());
    }
}