using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using MyWebApi.Domain.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Consumer;

[ExcludeFromCodeCoverage]
public sealed class OnOrderSubmittedConsumer : IConsumer<IOrderSubmitted>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OnOrderSubmittedConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IOrderSubmitted> context)
    {
        Console.WriteLine($"Processing OrderId {context.Message.OrderId}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"OrderId {context.Message.OrderId} Submitted");

        await _publishEndpoint.Publish<IOrderProcessed>(new OrderProcessed
        {
            OrderId = context.Message.OrderId
        });
    }
}
