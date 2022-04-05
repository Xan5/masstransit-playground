using System;
using MassTransit;

namespace Events;

[EntityName("payment-failed")]
public interface PaymentFailed
{
    Guid OrderId { get; }
    string Reason { get; }
}