using System;
using MassTransit;

namespace Events;

[EntityName("amount-provisioned")]
public interface AmountProvisioned
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
    decimal Amount { get; }
    DateTimeOffset ProvisioningExpiration { get; }
}