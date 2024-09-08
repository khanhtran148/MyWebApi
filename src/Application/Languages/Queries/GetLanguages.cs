using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Application.Abstractions;
using MyWebApi.Domain.Common;
using MyWebApi.Domain.Entities;

namespace MyWebApi.Application.Languages.Queries
{
    public static class GetLanguages
    {
        public sealed class Request : IRequest<Result<List<Select2ItemResponse>>>
        {
            public string Query { get; set; } = string.Empty;
            public List<string> Ids { get; set; } = new();
            public bool IncludeDisabled { get; set; } = true;
        }

        public sealed class GetLanguagesHandler : IRequestHandler<Request, Result<List<Select2ItemResponse>>>
        {
            private readonly IMyDbContext _productivityContext;

            public GetLanguagesHandler(IMyDbContext productivityContext)
            {
                _productivityContext = productivityContext;
            }

            public async Task<Result<List<Select2ItemResponse>>> Handle(Request request, CancellationToken cancellationToken)
            {
                IQueryable<Language> query = _productivityContext.Languages.AsNoTracking();

                if (!request.IncludeDisabled)
                    query = query.Where(x => !x.Disabled);

                if (request.Ids != null && request.Ids.Any())
                    query = query.Where(x => request.Ids.Contains(x.Id));
                else if (!string.IsNullOrWhiteSpace(request.Query))
                    query = query.Where(x => (x.Label != null && x.Label.Trim().ToLower().Contains(request.Query.Trim().ToLower()))
                                             || x.Id.Contains(request.Query.Trim().ToUpper()));

                // Projection query
                List<Select2ItemResponse> data = await query
                    .Select(x => new Select2ItemResponse()
                    {
                        Id = x.Id,
                        Text = x.Label,
                        Disabled = x.Disabled
                    }).ToListAsync(cancellationToken);

                //using mapster
                //List<Select2ItemResponse> data2 = await query.ProjectToType<Select2ItemResponse>().ToListAsync(cancellationToken);

                return Result.Success(data);
            }
        }
    }
}
