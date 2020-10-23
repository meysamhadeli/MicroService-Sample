using System;
using MicroPack.CQRS.Queries;
using Pacco.Services.Availability.Application.DTO;

namespace Pacco.Services.Availability.Application.Queries
{
    public class GetResource : IQuery<ResourceDto>
    {
        public Guid ResourceId { get; set; }
    }
}