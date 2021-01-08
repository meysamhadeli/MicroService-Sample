using System;
using System.Threading.Tasks;
using Pacco.Services.Availability.Api;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Tests.Shared.Factories;
using Pacco.Services.Availability.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Pacco.Services.Availability.Tests.Integration.Async
{
    public class AddResourceTests: IDisposable,IClassFixture<PaccoApplicationFactory<Startup>>
    {
        private readonly MongoDbFixture<ResourceDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;
        private const string Exchange = "availability";
        
        private Task Act(AddResource command) => _rabbitMqFixture.PublishAsync(command,Exchange);

        [Fact]
        public async Task add_resource_command_should_add_document_with_given_id_to_database()
        {
            var command = new AddResource(Guid.NewGuid(), new[] {"tag"});
            
            var tcs = _rabbitMqFixture.SubscribeAndGet<ResourceAdded, ResourceDocument>(Exchange,
                _mongoDbFixture.GetAsync, command.ResourceId);
            
            await Act(command);

            var document = await tcs.Task;
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ResourceId);
            document.Tags.ShouldBe(command.Tags);
        }
        
            
        #region Arrange
        
        public AddResourceTests(PaccoApplicationFactory<Startup> factory)
        {
            factory.Server.AllowSynchronousIO = true;
            _mongoDbFixture = new MongoDbFixture<ResourceDocument, Guid>("resources");
            _rabbitMqFixture = new RabbitMqFixture(Exchange);
        }

        
        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }
        #endregion
    }
}