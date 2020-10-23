using System;
using System.Collections.Generic;
using System.Linq;
using Pacco.Services.Availability.Core.Entities;

namespace Pacco.Services.Availability.Application.DTO
{
    public class ResourceDto
    {
        public Guid Id { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<ReservationDto> Reservations { get; set; }

        public static ResourceDto FromEntity(Resource entity) => new ResourceDto
        {
            Id = entity.Id,
            Reservations = entity.Reservations.Select(x =>
            new ReservationDto()
            {
                DateTime = x.DateTime,
                Priority = x.Priority
            }),
            Tags = entity.Tags
        };

    }
}