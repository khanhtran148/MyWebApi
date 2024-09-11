using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using MyWebApi.Domain.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Consumer;

[ExcludeFromCodeCoverage]
public sealed class OrderProcessConsumer : IConsumer<IOrderProcess>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderProcessConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IOrderProcess> context)
    {
        Console.WriteLine($"Processing OrderId {context.Message.OrderId}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"OrderId {context.Message.OrderId} Processed");

        await _publishEndpoint.Publish<IOrderSentMail>(new OrderSentMail
        {
            OrderId = context.Message.OrderId
        });
    }
}
