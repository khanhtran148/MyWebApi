using System;
using MassTransit;

namespace MyWebApi.Domain.Entities.Saga;

public class Monitoring : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public int CurrentState { get; set; }
    public int OrderId { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}
