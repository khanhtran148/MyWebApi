using Mapster;

namespace MyWebApi.Api.Server.MappingProfiles;

public sealed class CommonMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.NewConfig<From, To>();
    }
}
