using System;

namespace Events;

public interface BuyOrderPlaced
{
    Guid OrderId { get; }
    Guid CustomerId { get; }
}