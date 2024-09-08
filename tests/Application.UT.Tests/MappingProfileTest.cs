using MapsterMapper;
using MyWebApi.Application;

namespace Application.UT.Tests;

public sealed class MappingProfileTest
{
    private readonly IMapper _mapper;

    public MappingProfileTest()
    {
        _mapper = new Mapper();
        _mapper.Config.Scan(
            typeof(ApplicationDependencyInjection).Assembly,
            typeof(MyWebApi.Infrastructure.InfrastructureDependencyInjection).Assembly);
    }
}
