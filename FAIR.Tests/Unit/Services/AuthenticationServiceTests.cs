using AutoMapper;
using FAIR.Application.DTOs.Identity;
using FAIR.Application.Exceptions;
using FAIR.Application.Mapping;
using FAIR.Application.Services.Implementations;
using FAIR.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace FAIR.Tests.Unit.Services
{
    public class AuthenticationServiceTests
    {
        private readonly IMapper _mapper;

        public AuthenticationServiceTests()
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();
        }

        [Fact]
        public async Task CreateUser_WhenValidationFails_ShouldThrowServiceValidationException()
        {
            var repositoryManager = new Mock<IRepositoryManager>();
            var registerValidator = new InlineValidator<Register>();
            registerValidator.RuleFor(x => x).Custom((_, context) => context.AddFailure("Email", "Invalid email"));
            var loginValidator = new InlineValidator<Login>();

            var service = new AuthenticationService(repositoryManager.Object, _mapper, registerValidator, loginValidator);

            var action = async () => await service.CreateUser(new Register
            {
                Username = "u",
                Password = "p",
                Email = "bad",
                FullName = "name",
                Role = "athlete",
                DateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-20)),
                Location = "loc"
            });

            await action.Should().ThrowAsync<ServiceValidationException>();
        }
    }
}
