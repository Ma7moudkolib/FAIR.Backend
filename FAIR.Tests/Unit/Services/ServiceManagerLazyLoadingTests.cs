using AutoMapper;
using FAIR.Application.DTOs.Identity;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.DTOs.Search;
using FAIR.Application.DTOs.Video;
using FAIR.Application.Mapping;
using FAIR.Application.Services.Implementations;
using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Managers;
using FAIR.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace FAIR.Tests.Unit.Services
{
    public class ServiceManagerLazyLoadingTests
    {
        [Fact]
        public void ServiceManager_ShouldResolveLazily()
        {
            var services = new ServiceCollection();

            var repositoryManager = new Mock<IRepositoryManager>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingConfig>()).CreateMapper();

            services.AddSingleton(repositoryManager.Object);
            services.AddSingleton<IMapper>(mapper);
            services.AddSingleton<IAiVideoService>(new Mock<IAiVideoService>().Object);
            services.AddSingleton<IValidator<Register>>(new InlineValidator<Register>());
            services.AddSingleton<IValidator<Login>>(new InlineValidator<Login>());
            services.AddSingleton<IValidator<UpdateAthleteProfile>>(new InlineValidator<UpdateAthleteProfile>());
            services.AddSingleton<IValidator<ChangePasswordRequest>>(new InlineValidator<ChangePasswordRequest>());
            services.AddSingleton<IValidator<AthleteSearchFilter>>(new InlineValidator<AthleteSearchFilter>());
            services.AddSingleton<IValidator<UpdateCoachProfile>>(new InlineValidator<UpdateCoachProfile>());
            services.AddSingleton<IValidator<VideoUploadDto>>(new InlineValidator<VideoUploadDto>());

            var provider = services.BuildServiceProvider();
            var manager = new ServiceManager(provider);

            manager.Should().NotBeNull();
            repositoryManager.Invocations.Should().BeEmpty();

            var athleteService1 = manager.AthleteService;
            var athleteService2 = manager.AthleteService;
            athleteService1.Should().NotBeNull();
            athleteService2.Should().BeSameAs(athleteService1);
        }
    }
}
