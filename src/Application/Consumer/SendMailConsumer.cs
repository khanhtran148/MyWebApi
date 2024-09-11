using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using MyWebApi.Domain.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Consumer;

[ExcludeFromCodeCoverage]
public sealed class SendMailConsumer : IConsumer<ISendMail>
{
    private readonly IPublishEndpoint _publishEndpoint;

    public SendMailConsumer(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<ISendMail> context)
    {
        Console.WriteLine($"Sending mail for OrderId {context.Message.OrderId}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"OrderId {context.Message.OrderId} mail sent");

        await _publishEndpoint.Publish(new MailSent() { OrderId = context.Message.OrderId });
    }
}
