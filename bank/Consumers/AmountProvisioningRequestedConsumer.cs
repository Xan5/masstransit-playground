using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bank.Consumers;

public class AmountProvisioningRequestedConsumer : IConsumer<AmountProvisioningRequested>
{
    private readonly ILogger<AmountProvisioningRequestedConsumer> _logger;

    public AmountProvisioningRequestedConsumer(
        ILogger<AmountProvisioningRequestedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AmountProvisioningRequested> context)
    {
        _logger.LogInformation(
            "Provisioning {Amount} for {Order}",
            context.Message.Amount,
            context.Message.OrderId);

        await context.Publish<AmountProvisioned>(new
        {
            context.Message.OrderId,
            context.Message.Amount,
            context.Message.CustomerId,
            ProvisioningExpiration = DateTimeOffset.Now.Add(context.Message.ProvisioningDuration),
        });
    }
}