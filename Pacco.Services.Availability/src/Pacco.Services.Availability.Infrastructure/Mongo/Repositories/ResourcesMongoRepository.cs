using System;
using System.Threading.Tasks;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;
using System.Collections.Generic;
using MicroPack.Mongo;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Repositories
{
    internal sealed class ResourcesMongoRepository : IResourcesRepository
    {
        private readonly IMongoRepository<ResourceDocument, Guid> _repository;

        public ResourcesMongoRepository(IMongoRepository<ResourceDocument, Guid> repository)
            => _repository = repository;

        public async Task<Resource> GetAsync(Guid id)
        {
            var document = await _repository.GetAsync(r => r.Id == id);
            return document?.AsEntity();
        }

        public Task<bool> ExistsAsync(Guid id)
            => _repository.ExistsAsync(r => r.Id == id);

        public Task AddAsync(Resource resource)
            => _repository.AddAsync(resource.AsDocument());

        public Task UpdateAsync(Resource resource)
            => _repository.UpdateAsync(resource.AsDocument());

        public Task DeleteAsync(Guid id)
            => _repository.DeleteAsync(id);

        public Task<IEnumerable<Resource>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}