using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Api.Server.Models.Languages;
using MyWebApi.Application.Languages.Queries;
using MyWebApi.Domain.Common;
using MyWebApi.Domain.Constants;

namespace MyWebApi.Api.Server.Controllers.V2
{
    [ApiVersion(2.0)]
    [Route("api/[controller]")]
    [ApiController]
    public sealed class SomethingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SomethingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = MyConstants.PrefixController + nameof(GetLanguages))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Select2ItemResponse>))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLanguages([FromQuery] GetRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Result<List<Select2ItemResponse>> result = await _mediator.Send(new GetLanguages.Request()
                {
                    Ids = request.Ids,
                    Query = request.Query,
                    IncludeDisabled = true
                }, cancellationToken);

                if (result.IsSuccess && result.Value != null)
                    return Ok(result.Value);

                return Ok(new List<Select2ItemResponse>());
            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: StatusCodes.Status422UnprocessableEntity);
            }
        }
    }
}
