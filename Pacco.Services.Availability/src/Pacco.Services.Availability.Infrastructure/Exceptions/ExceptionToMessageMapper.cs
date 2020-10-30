using System;
using MicroPack.MessageBrokers.RabbitMQ;
using Pacco.Services.Availability.Application.Events.Rejected;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Core.Exceptions;

namespace Pacco.Services.Availability.Infrastructure.Exceptions
{
    public class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message)
            => exception switch
            {
                CannotExpropriateReservationException ex => new ReserveResourceRejected(ex.Message,ex.Code,ex.ResourceId),
                ResourceAlreadyExistsException ex => new AddResourceRejected(ex.Id, ex.Message, ex.Code),
                ResourceNotFoundException ex => new ReserveResourceRejected(ex.Message,ex.Code,ex.Id),
                _ => null
            };
    }
}