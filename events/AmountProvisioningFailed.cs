using System;
using MassTransit;

namespace Events;

[EntityName("amount-provisioning-failed")]
public interface AmountProvisioningFailed
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
}