using System;
using MassTransit;

namespace Shop.Sagas;

public class OrderPurchasingState : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = nameof(OrderPurchasingStateMachine.Initial);
    public Guid? CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public decimal? OrderAmount { get; set; }
    public bool? IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public int Version { get; set; }
}