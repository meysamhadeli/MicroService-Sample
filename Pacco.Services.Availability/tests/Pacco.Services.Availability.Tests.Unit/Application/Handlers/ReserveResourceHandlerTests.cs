using System;
using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using NSubstitute;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Commands.Handlers;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Application.Services.Clients;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;
using Shouldly;
using Xunit;

namespace Pacco.Services.Availability.Tests.Unit.Application.Handlers
{
    public class ReserveResourceHandlerTests
    {
        private readonly IResourcesRepository _resourceRepository;
        private readonly IEventProcessor _eventProcessor;
        private readonly ICustomersServiceClient _customerServiceClient;
        private readonly ICommandHandler<ReserveResource> _handler;

        private Task Act(ReserveResource command) => _handler.HandleAsync(command);

        #region Arrange

        public ReserveResourceHandlerTests()
        {
            _resourceRepository = Substitute.For<IResourcesRepository>();
            _eventProcessor = Substitute.For<IEventProcessor>();
            _customerServiceClient = Substitute.For<ICustomersServiceClient>();

            _handler = new ReserveResourceHandler(_resourceRepository, _customerServiceClient, _eventProcessor);
        }

        #endregion

        [Fact]
        public async Task given_invalid_id_reserve_resource_should_throw_an_exception()
        {
            //Act
            var command = new ReserveResource(Guid.NewGuid(), DateTime.Now, 2, Guid.NewGuid());
            var exception = await Record.ExceptionAsync(async () => await Act(command));
            //Assert
            exception.ShouldBeOfType<ResourceNotFoundException>();
        }

        [Fact]
        public async Task given_valid_resource_Id_for_valid_customer_reserve_resource_should_be_success()
        {
            //Act
            var command = new ReserveResource(Guid.NewGuid(), DateTime.Now, 2, Guid.NewGuid());
            var resource = new Resource(command.CustomerId, new[] {"tag"});
            _resourceRepository.GetAsync(command.ResourceId).Returns(resource);
            var customerStateDto = new CustomerStateDto() {State = "valid"};
            _customerServiceClient.GetStateAsync(command.CustomerId).Returns(customerStateDto);

            await Act(command);
            //Assert
            await _resourceRepository.Received().UpdateAsync(resource);
            await _eventProcessor.Received().ProcessAsync(resource.Events);
        }
    }
}