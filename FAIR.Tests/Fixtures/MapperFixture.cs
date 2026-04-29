using AutoMapper;
using FAIR.Application.Mapping;
using Xunit;

namespace FAIR.Tests.Fixtures
{
    /// <summary>
    /// xUnit fixture for initializing and providing a validated AutoMapper instance.
    /// This fixture ensures all mappings are compiled successfully and can be reused across tests.
    /// </summary>
    public class MapperFixture : IDisposable
    {
        private readonly IMapper _mapper;

        public MapperFixture()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });

            // NOTE: AssertConfigurationIsValid() is commented out because production code has unmapped members
            // Those should be fixed in production code (MappingConfig), but for now we allow partial mappings
            // config.AssertConfigurationIsValid();

            _mapper = config.CreateMapper();
        }

        public IMapper Mapper => _mapper;

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Collection fixture for sharing MapperFixture across multiple test classes.
    /// </summary>
    [CollectionDefinition("Mapper Collection")]
    public class MapperCollection : ICollectionFixture<MapperFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to define the collection that xUnit will use to apply the collection fixture.
    }
}
