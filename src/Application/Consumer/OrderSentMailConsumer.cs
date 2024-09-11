using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using MyWebApi.Domain.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Consumer;

[ExcludeFromCodeCoverage]
public sealed class OrderSentMailConsumer : IConsumer<IOrderSentMail>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderSentMailConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<IOrderSentMail> context)
    {
        Console.WriteLine($"Sending mail for OrderId {context.Message.OrderId}");
        await Task.Delay(TimeSpan.FromSeconds(5));
        Console.WriteLine($"OrderId {context.Message.OrderId} mail sent");

        await _publishEndpoint.Publish<IMailSent>(new MailSent
        {
            OrderId = context.Message.OrderId
        });
    }
}
