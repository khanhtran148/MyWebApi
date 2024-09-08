﻿using System;
using MassTransit;
using Microsoft.Extensions.Logging;
using MyWebApi.Domain.Entities.Saga;
using MyWebApi.Messages.Orders;

namespace MyWebApi.Application.Saga;

public class MyStateMachine : MassTransitStateMachine<Monitoring>
{
    // define the state machine
    public State Submitted { get; private set; }
    public State Processed { get; private set; }
    public State MailSent { get; private set; }

    // define event
    public Event<IOrderSubmitted> OrderSubmitted { get; private set; }
    public Event<IOrderProcessed> OrderProcessed { get; private set; }
    public Event<IMailSent> OrderMailSent { get; private set; }

    public MyStateMachine(ILogger<MyStateMachine> logger)
    {
        logger.LogInformation("MyStateMachine => Initially");

        // State data : 0 - None, 1 - Initial, 2 - Final, 3 - Submitted, 4 - Processed, 5 - MailSent
        InstanceState(x => x.CurrentState, Submitted, Processed, MailSent);

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.SubmittedAt = DateTime.UtcNow;
                    context.Saga.CreatedDate = DateTime.UtcNow;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                    logger.LogInformation("MyStateMachine => OrderSubmitted");
                })
                .TransitionTo(Submitted),

            When(OrderProcessed)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                })
                .TransitionTo(Processed),

            When(OrderMailSent)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                })
                .TransitionTo(MailSent)
        );

        Event(() => OrderSubmitted, x =>
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

        Event(() => OrderProcessed, x =>
        {
            x.CorrelateBy((instant, context) => instant.OrderId == context.Message.OrderId);
        });

        Event(() => OrderMailSent, x =>
        {
            x.CorrelateBy((instant, context) => instant.OrderId == context.Message.OrderId);
        });

        During(Submitted,
            Ignore(OrderMailSent),
            When(OrderProcessed)
                .Then(context => logger.LogInformation("CorrelationId: {CandidateDataDeletedStage} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Message.OrderId, context.Saga.CurrentState))
                .TransitionTo(MailSent)
            );

        During(MailSent,
            Ignore(OrderProcessed),
            When(OrderMailSent)
                .Then(context =>
                {
                    context.Saga.FinishedAt = DateTime.UtcNow;
                    context.Saga.UpdatedDate = DateTime.UtcNow;
                    logger.LogInformation("CorrelationId: {CandidateDataDeletedStage} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                        context.Saga.CorrelationId, context.Message.OrderId, context.Saga.CurrentState);
                })
                .Finalize()
                .Then(context => logger.LogInformation("[DONE] CorrelationId: {CandidateDataDeletedStage} - OrderId: {OrderId} - CurrentState: {CurrentState}",
                    context.Saga.CorrelationId, context.Message.OrderId, context.Saga.CurrentState))
        );

        During(Final,
            Ignore(OrderProcessed),
            Ignore(OrderMailSent)
        );
    }
}
