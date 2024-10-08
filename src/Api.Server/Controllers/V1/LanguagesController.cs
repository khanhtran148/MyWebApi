﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CSharpFunctionalExtensions;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Api.Server.Models.Languages;
using MyWebApi.Application.Abstractions;
using MyWebApi.Application.Languages.Queries;
using MyWebApi.Domain.Common;
using MyWebApi.Domain.Constants;
using MyWebApi.Domain.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Api.Server.Controllers.V1
{
    [ApiVersion(1.0)]
    [Route("api/[controller]")]
    [ApiController]
    public sealed class LanguagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMonitoringDbContext _dbContext;

        public LanguagesController(IMediator mediator,
            IPublishEndpoint publishEndpoint,
            IMonitoringDbContext dbContext)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _dbContext = dbContext;
        }

        [HttpGet(Name = MyConstants.PrefixController + nameof(GetLanguages))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Select2ItemResponse>))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetLanguages([FromQuery] GetRequest request, CancellationToken cancellationToken)
        {
            Result<List<Select2ItemResponse>> result = await _mediator.Send(new GetLanguages.Request() { Ids = request.Ids, Query = request.Query, IncludeDisabled = true }, cancellationToken);

            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);

            return Ok(new List<Select2ItemResponse>());
        }

        [HttpGet("{id}", Name = MyConstants.PrefixController + nameof(GetById))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Select2ItemResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            Result<Select2ItemResponse> result = await _mediator.Send(new GetById.Request(id), cancellationToken);

            return result
                .Finally(r => r.IsSuccess ? (IActionResult)Ok(new Select2ItemResponse(r.Value.Id, r.Value.Text, r.Value.Disabled)) : NotFound());
        }

        [HttpPost("PublishMessage", Name = MyConstants.PrefixController + nameof(PublishMessage))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PublishMessage(int orderId, CancellationToken cancellationToken)
        {
            await _publishEndpoint.Publish<IOrderProcess>(new OrderProcess() { OrderId = orderId }, cancellationToken);
            return Ok();
        }

        [HttpPost("PublishMultiMessages", Name = MyConstants.PrefixController + nameof(PublishMultiMessages))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PublishMultiMessages(int nbOfMessage, CancellationToken cancellationToken)
        {
            int currentMaxOrderId = 0;
            if (await _dbContext.Monitorings.AnyAsync(cancellationToken))
            {
                currentMaxOrderId = await _dbContext.Monitorings
                    .MaxAsync(x => x.OrderId, cancellationToken);
            }

            for (int i = 1; i < nbOfMessage + 1; i++)
            {
                await _publishEndpoint.Publish<IOrderProcess>(new OrderProcess() { OrderId = currentMaxOrderId + i }, cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }

            return Ok();
        }
    }
}
