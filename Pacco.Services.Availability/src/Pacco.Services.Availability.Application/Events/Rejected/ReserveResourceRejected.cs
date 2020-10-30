using System;
using MicroPack.CQRS.Events;

namespace Pacco.Services.Availability.Application.Events.Rejected
{
    [Contract]
    public class ReserveResourceRejected: IRejectedEvent
    {
        public Guid ResourceId { get; }
        public string Reason { get; }
        public string Code { get; }
        
        public ReserveResourceRejected(string reason, string code, Guid resourceId)
        {
            Reason = reason;
            Code = code;
            ResourceId = resourceId;
        }
    }
}