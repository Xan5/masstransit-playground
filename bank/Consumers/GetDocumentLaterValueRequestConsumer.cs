using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bank.Consumers;

public class GetDocumentLaterValueRequestConsumer : IConsumer<GetDocumentLaterValueRequest>
{
    private readonly ILogger<GetDocumentLaterValueRequestConsumer> _logger;

    public GetDocumentLaterValueRequestConsumer(ILogger<GetDocumentLaterValueRequestConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetDocumentLaterValueRequest> context)
    {
        _logger.LogInformation("Consuming {Message}", nameof(GetDocumentLaterValueRequest));

        await context.RespondAsync<GetDocumentLaterValueResponse>(new Response(DateTimeOffset.Now.Millisecond));
    }

    private class Response : GetDocumentLaterValueResponse
    {
        public Response(int laterValue)
        {
            LaterValue = laterValue;
        }

        public int LaterValue { get; }
    }
}