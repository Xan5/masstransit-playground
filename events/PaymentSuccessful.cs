using System;
using MassTransit;

namespace Events;

[EntityName("payment-successful")]
public interface PaymentSuccessful
{
    Guid OrderId { get; }
}