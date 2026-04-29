using AutoMapper;
using FAIR.Tests.Fixtures;
using FAIR.Tests.Mocks;
using Xunit;

namespace FAIR.Tests.Common
{
    /// <summary>
    /// Base class for service tests.
    /// Provides common setup with mock managers and mapper.
    /// </summary>
    public abstract class ServiceTestBase : IClassFixture<MapperFixture>
    {
        protected readonly MapperFixture MapperFixture;
        protected readonly MockRepositoryManager MockRepositoryManager;
        protected readonly MockServiceManager MockServiceManager;

        protected IMapper Mapper => MapperFixture.Mapper;

        protected ServiceTestBase(MapperFixture mapperFixture)
        {
            MapperFixture = mapperFixture;
            MockRepositoryManager = new MockRepositoryManager();
            MockServiceManager = new MockServiceManager(mapperFixture);
        }
    }
}
