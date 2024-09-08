using System.Diagnostics.CodeAnalysis;
using Mapster;
using MyWebApi.Domain.Common;
using MyWebApi.Domain.Entities;

namespace MyWebApi.Application.Languages;

[ExcludeFromCodeCoverage]
public sealed class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Language, Select2ItemResponse>()
            .Map(x => x.Id, x => x.Id)
            .Map(x => x.Disabled, x => x.Disabled)
            .Map(x => x.Text, x => x.Label);
    }
}
