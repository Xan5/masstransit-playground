using System;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shop.Sagas.Activities;

namespace Shop.Sagas;

public class OrderPurchasingStateMachine : MassTransitStateMachine<OrderPurchasingState>
{
    private readonly ILogger<OrderPurchasingStateMachine> _logger;

    public OrderPurchasingStateMachine(
        ILogger<OrderPurchasingStateMachine> logger)
    {
        _logger = logger;

        InstanceState(instance => instance.CurrentState);

        Event(
            () => BuyOrderPlaced,
            @event => @event.CorrelateById(context => context.Message.OrderId));

        Event(
            () => AmountProvisioned,
            @event => @event.CorrelateById(context => context.Message.OrderId));

        Event(
            () => PaymentSuccessful,
            @event => @event.CorrelateById(context => context.Message.OrderId));
        Event(
            () => ProvisioningFailed,
            @event => @event.CorrelateById(context => context.Message.OrderId));
        Event(
            () => PaymentFailed,
            @event => @event.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(BuyOrderPlaced)
               .Then(context => _logger.LogInformation(
                         "{Customer} placed {Order}",
                         context.Message.CustomerId,
                         context.Message.OrderId))
               .Then(context =>
                     {
                         context.Saga.CustomerId = context.Message.CustomerId;
                         context.Saga.OrderId = context.Message.OrderId;
                     })
               .Activity(context => context.OfType<GetOrderAmount>())
               .PublishAsync(context => context.Init<AmountProvisioningRequested>(new
                {
                    OrderId = context.Saga.OrderId.Value,
                    CustomerId = context.Saga.CustomerId.Value,
                    Amount = context.Saga.OrderAmount.Value,
                    ProvisioningDuration = TimeSpan.FromMinutes(15),
                }))
               .TransitionTo(AwaitingProvisioning));

        During(
            AwaitingProvisioning,
            When(AmountProvisioned)
               .Then(context => _logger.LogInformation(
                         "Provisioning of {Amount} from {Customer} for {Order} confirmed",
                         context.Message.Amount,
                         context.Message.CustomerId,
                         context.Message.OrderId))
               .TransitionTo(AwaitingPayment),
            When(ProvisioningFailed)
               .Then(context => _logger.LogInformation(
                         "Provisioning of {Amount} from {Customer} for {Order} failed",
                         context.Saga.OrderAmount,
                         context.Saga.CustomerId,
                         context.Saga.OrderId))
               .TransitionTo(Cancelled));

        During(
            AwaitingPayment,
            When(PaymentSuccessful)
               .Finalize(),
            When(PaymentFailed)
               .TransitionTo(Cancelled));
    }

    public State AwaitingProvisioning { get; private set; }
    public State AwaitingPayment { get; private set; }
    public State Cancelled { get; private set; }
    public Event<BuyOrderPlaced> BuyOrderPlaced { get; private set; }
    public Event<AmountProvisioned> AmountProvisioned { get; private set; }
    public Event<PaymentSuccessful> PaymentSuccessful { get; private set; }
    public Event<AmountProvisioningFailed> ProvisioningFailed { get; private set; }
    public Event<PaymentFailed> PaymentFailed { get; private set; }
}