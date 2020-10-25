using System;

namespace Pacco.Services.Availability.Application.Exceptions
{
    public class ResourceAlreadyExistsException : AppException
    {
        public ResourceAlreadyExistsException(Guid id) : base($"Resource with id: {id} already exists.")
        {
        }
    }
}