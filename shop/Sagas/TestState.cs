using System;
using MassTransit;

namespace Shop.Sagas;

public class TestState : SagaStateMachineInstance, ISagaVersion
{
    public string CurrentState { get; set; } = nameof(TestStateMachine.Initial);
    public Guid CorrelationId { get; set; }
    public int Version { get; set; }
}
