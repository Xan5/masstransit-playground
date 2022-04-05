using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shop.Controllers;

[ApiController]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public BasketController(
        ILogger<BasketController> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("customers/{customerId:guid}/orders/{orderId:guid}")]
    public async Task<IActionResult> BuyAsync(Guid customerId, Guid orderId)
    {
        var amount = new decimal(new Random().Next() * 100);

        _logger.LogInformation(
            "{CustomerId} confirmed {OrderId}",
            customerId,
            orderId);

        await _publishEndpoint.Publish<BuyOrderPlaced>(
            new BuyOrderPlacedEvent(orderId, customerId));

        return Accepted();
    }

    private record BuyOrderPlacedEvent(Guid OrderId, Guid CustomerId) : BuyOrderPlaced;
}