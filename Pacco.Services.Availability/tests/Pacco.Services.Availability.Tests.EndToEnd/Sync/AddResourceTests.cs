using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pacco.Services.Availability.Api;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Tests.Shared.Factories;
using Pacco.Services.Availability.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Pacco.Services.Availability.Tests.EndToEnd.Sync
{
    public class AddResourceTests : IDisposable, IClassFixture<PaccoApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ResourceDocument, Guid> _mongoDbFixture;

        #region Arrange

        public AddResourceTests(PaccoApplicationFactory<Startup> factory)
        {
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
            _mongoDbFixture = new MongoDbFixture<ResourceDocument, Guid>("resources");
        }

        private static StringContent GetContent(object value)
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion

        private Task<HttpResponseMessage> Act(AddResource command)
            => _httpClient.PostAsync("api/resources", GetContent(command));

        [Fact]
        public async Task add_resource_endpoint_should_return_http_status_code_created()
        {
            var command = new AddResource(Guid.NewGuid(), new[] {"tag"});
            var response = await Act(command);
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task add_resource_endpoint_should_add_document_with_given_id_to_database()
        {
            var command = new AddResource(Guid.NewGuid(), new[] {"tag"});
            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.ResourceId);
            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ResourceId);
            document.Tags.ShouldBe(command.Tags);
        }
    }
}