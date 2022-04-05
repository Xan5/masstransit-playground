using System;
using MassTransit;

namespace Events;

[EntityName("amount-provisioning-requested")]
public interface AmountProvisioningRequested
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    decimal Amount { get; }
    TimeSpan ProvisioningDuration { get; }
}