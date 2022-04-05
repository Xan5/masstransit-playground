using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shop.Sagas.Activities;

public class GetOrderAmount : IStateMachineActivity<OrderPurchasingState, BuyOrderPlaced>
{
    private readonly ILogger<GetOrderAmount> _logger;

    public GetOrderAmount(ILogger<GetOrderAmount> logger)
    {
        _logger = logger;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(GetOrderAmount));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(
        BehaviorContext<OrderPurchasingState, BuyOrderPlaced> context,
        IBehavior<OrderPurchasingState, BuyOrderPlaced> next)
    {
        var amount = new decimal(Math.Round(new Random().NextDouble() * 100, 2));

        _logger.LogInformation(
            "{OrderId} is {Amount} due",
            context.Message.OrderId,
            amount);

        context.Saga.OrderAmount = amount;

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OrderPurchasingState, BuyOrderPlaced, TException> context,
        IBehavior<OrderPurchasingState, BuyOrderPlaced> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}