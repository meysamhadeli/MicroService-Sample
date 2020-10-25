using System;

namespace Pacco.Services.Availability.Application.Exceptions
{
    public class ResourceNotFoundException : AppException
    {
        public ResourceNotFoundException(Guid id) : base($"Resource with id: {id} Not Found.")
        {
        }
    }
}