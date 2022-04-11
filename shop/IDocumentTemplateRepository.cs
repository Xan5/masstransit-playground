using System;
using System.Threading;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shop;

public interface IDocumentTemplateRepository
{
    Task<DocumentTemplate?> GetById(Guid id, CancellationToken cancellationToken = default);
}

public class DocumentTemplate
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int LaterValue { get; set; }
}

public class FileStorageServiceBasedDocumentTemplateRepository : IDocumentTemplateRepository
{
    private readonly ILogger<FileStorageServiceBasedDocumentTemplateRepository> _logger;
    private readonly IRequestClient<GetDocumentTemplateMetadataRequest> _metadataClient;
    private readonly IRequestClient<GetDocumentLaterValueRequest> _laterValueClient;

    public FileStorageServiceBasedDocumentTemplateRepository(
        ILogger<FileStorageServiceBasedDocumentTemplateRepository> logger,
        IRequestClient<GetDocumentTemplateMetadataRequest> metadataClient,
        IRequestClient<GetDocumentLaterValueRequest> laterValueClient)
    {
        _logger = logger;
        _metadataClient = metadataClient;
        _laterValueClient = laterValueClient;
    }

    public async Task<DocumentTemplate?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Trying to get metadata");
        var document = await GetMetadataAsync(id, cancellationToken);
        _logger.LogInformation("Trying to get later value");
        var laterValue = await GetContentStreamAsync(id, cancellationToken);

        return new DocumentTemplate
        {
            Name = document.Name,
            LaterValue = laterValue,
        };
    }

    private async Task<int> GetContentStreamAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing Content event");

        var response = await _laterValueClient
           .GetResponse<GetDocumentLaterValueResponse>(
                new LaterValueRequest(id),
                cancellationToken);

        _logger.LogInformation("Content responded");

        return response.Message.LaterValue;
    }

    private async Task<DocumentMetadataDto> GetMetadataAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing metadata event");

        var resposne = await _metadataClient
           .GetResponse<GetDocumentTemplateMetadataResposne>(new MetadataRequest(id), cancellationToken);

        _logger.LogInformation("Metadata responded");
        return new DocumentMetadataDto
        {
            Name = resposne.Message.Name,
        };
    }

    private class LaterValueRequest : GetDocumentLaterValueRequest
    {
        public LaterValueRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    private class MetadataRequest : GetDocumentTemplateMetadataRequest
    {
        public MetadataRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}

public class DocumentMetadataDto
{
    public string Name { get; set; }
}