using System;
using MassTransit;
using Microsoft.Extensions.Logging;
using MyWebApi.Domain.Entities.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Saga;

public class MyStateMachine : MassTransitStateMachine<Monitoring>
{
    // define the state
    public State Processed { get; private set; }
    public State MailSent { get; private set; }

    // define event
    public Event<IOrderProcess> OrderProcess { get; private set; }
    public Event<IOrderSentMail> OrderSentMail { get; private set; }
    public Event<IMailSent> OrderMailAlreadySent { get; private set; }

    public MyStateMachine(ILogger<MyStateMachine> logger)
    {
        logger.LogInformation("MyStateMachine => Initially");

        // State data : 0 - None, 1 - Initial, 2 - Final, 3 - Processed, 4 - MailSent
        InstanceState(x => x.CurrentState, Processed, MailSent);

        Initially(
            When(OrderProcess)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.SubmittedAt = DateTime.UtcNow;
                    context.Saga.CreatedDate = DateTime.UtcNow;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                    logger.LogInformation("MyStateMachine => OrderSubmitted");
                })
                .TransitionTo(Initial),
            When(OrderSentMail)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                }),
            When(OrderMailAlreadySent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                })
        );

        Event(() => OrderProcess, x =>
        {
            x.CorrelateBy((instant, context) => instant.OrderId == context.Message.OrderId);
            x.SelectId(e => NewId.NextGuid());
            x.InsertOnInitial = true;
            x.SetSagaFactory(context => new Monitoring
            {
                CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
                OrderId = context.Message.OrderId,
                SubmittedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            });
        });

        Event(() => OrderSentMail, x => { x.CorrelateBy((instant, context) => instant.OrderId == context.Message.OrderId); });

        Event(() => OrderMailAlreadySent, x => { x.CorrelateBy((instant, context) => instant.OrderId == context.Message.OrderId); });

        During(Initial,
            Ignore(OrderMailAlreadySent),
            When(OrderSentMail)
                .Then(context => logger.LogInformation("CorrelationId: {CorrelationId} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Message.OrderId, context.Saga.CurrentState))
                .TransitionTo(Processed)
        );

        During(Processed,
            When(OrderMailAlreadySent)
                .Then(context => logger.LogInformation("CorrelationId: {CorrelationId} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Message.OrderId, context.Saga.CurrentState))
                .TransitionTo(MailSent)
        );

        During(MailSent,
            Ignore(OrderSentMail),
            Ignore(OrderMailAlreadySent),
            When(MailSent.Enter)
                .Then(context => logger.LogInformation("CorrelationId: {CorrelationId} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Saga.OrderId, context.Saga.CurrentState))
                .Finalize()
        );

        During(Final,
            Ignore(OrderSentMail),
            Ignore(OrderMailAlreadySent),
            When(Final.Enter)
                .Then(context => logger.LogInformation("[DONE] CorrelationId: {CorrelationId} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Saga.OrderId, context.Saga.CurrentState))
        );
    }
}
