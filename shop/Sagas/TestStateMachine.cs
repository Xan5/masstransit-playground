using System;
using DnsClient.Internal;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shop.Sagas.Activities;

namespace Shop.Sagas;

public class TestStateMachine : MassTransitStateMachine<TestState>
{
    private readonly ILogger<TestStateMachine> _logger;

    public TestStateMachine(ILogger<TestStateMachine> logger)
    {
        _logger = logger;
        InstanceState(instance => instance.CurrentState);

        Event(
            () => StartTestSaga,
            @event => @event.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(StartTestSaga)
               .Then(context => _logger.LogInformation(
                         "received StartTestSaga"))
               .Activity(context => context.OfType<TestActivity>())
               .Then(context => _logger.LogInformation(
                         "received response"))
               .TransitionTo(ResponseReceived));
    }

    public State ResponseReceived { get; private set; }
    public Event<StartTestSaga> StartTestSaga { get; private set; }
}
