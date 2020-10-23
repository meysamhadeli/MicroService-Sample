using System.Collections.Generic;
using MicroPack.CQRS.Queries;
using Pacco.Services.Availability.Application.DTO;

namespace Pacco.Services.Availability.Application.Queries
{
    public class GetResources : IQuery<IEnumerable<ResourceDto>>
    {
        public IEnumerable<string> Tags { get; set; }
        public bool MatchAllTags { get; set; }
    }
}