using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Application.Abstractions;
using MyWebApi.Domain.Common;

namespace MyWebApi.Application.Languages.Queries
{
    public static class GetById
    {
        public sealed class Request : IRequest<Result<Select2ItemResponse>>
        {
            public Request(string id)
            {
                Id = id;
            }

            public string Id { get; set; }
        }

        public sealed class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage("Id is invalid");
            }
        }

        public sealed class GetByIdHandler : IRequestHandler<Request, Result<Select2ItemResponse>>
        {
            private readonly IMyDbContext _productivityContext;

            public GetByIdHandler(IMyDbContext productivityContext)
            {
                _productivityContext = productivityContext;
            }

            public async Task<Result<Select2ItemResponse>> Handle(Request request, CancellationToken cancellationToken)
            {
                Select2ItemResponse data = await _productivityContext.Languages
                    .Where(x => request.Id == x.Id)
                    .Select(x => new Select2ItemResponse()
                    {
                        Id = x.Id,
                        Text = x.Label,
                        Disabled = x.Disabled
                    }).FirstOrDefaultAsync(cancellationToken);

                if (data is null) return Result.Failure<Select2ItemResponse>("No data found");

                return Result.Success(data);
            }
        }
    }
}
