using System.Collections.Generic;
using MyWebApi.Api.Server.Models.Common;

namespace MyWebApi.Api.Server.Models.Languages
{
    public sealed class GetRequest : BaseFilterRequest
    {
        public List<string> Ids { get; set; }
    }
}
