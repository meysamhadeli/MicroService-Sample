using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;
using System.Collections.Generic;
using MicroPack.Mongo;

namespace Pacco.Services.Availability.Infrastructure.Mongo.Repositories
{
    public class ResourcesMongoRepository : IResourcesRepository
    {
        private readonly IMongoRepository<Resource, Guid> _mongoRepository;

        public ResourcesMongoRepository(IMongoRepository<Resource, Guid> mongoRepository){
            _mongoRepository = mongoRepository;
        }
            
        public async Task<Resource> GetAsync(AggregateId id)
        {
            var document = await _mongoRepository.GetAsync(r => r.Id == id);
            return document;
        }

         public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            var document = await _mongoRepository.FindAsync(_=> true);
            return document;
        }

        public Task<bool> ExistsAsync(AggregateId id)
            => _mongoRepository.ExistsAsync(r => r.Id == id);

        public Task AddAsync(Resource resource)
            => _mongoRepository.AddAsync(resource);

        public Task UpdateAsync(Resource resource)
            => _mongoRepository.Collection.ReplaceOneAsync(r => r.Id == resource.Id && r.Version < resource.Version,resource);

        public Task DeleteAsync(AggregateId id)
            => _mongoRepository.DeleteAsync(id);
    }
}