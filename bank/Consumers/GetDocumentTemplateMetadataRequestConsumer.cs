using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bank.Consumers;

public class GetDocumentTemplateMetadataRequestConsumer : IConsumer<GetDocumentTemplateMetadataRequest>
{
    private readonly ILogger<GetDocumentTemplateMetadataRequestConsumer> _logger;

    public GetDocumentTemplateMetadataRequestConsumer(ILogger<GetDocumentTemplateMetadataRequestConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetDocumentTemplateMetadataRequest> context)
    {
        _logger.LogInformation("Consuming {Message}", nameof(GetDocumentTemplateMetadataRequest));

        await context.RespondAsync<GetDocumentTemplateMetadataResposne>(new Response(Guid.NewGuid(), "Dupa jasia"));
    }

    private class Response : GetDocumentTemplateMetadataResposne
    {
        public Response(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
    }
}