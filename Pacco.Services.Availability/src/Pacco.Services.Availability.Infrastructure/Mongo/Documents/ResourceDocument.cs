using System;
using System.Collections.Generic;
using MicroPack.MicroPack.Types;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Documents
{
    public class ResourceDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public long Version { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<ReservationDocument> Reservations { get; set; }
    }
}