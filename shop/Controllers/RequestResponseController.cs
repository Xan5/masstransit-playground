using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shop.Controllers;

public class RequestResponseController : ControllerBase
{
    private readonly ILogger<RequestResponseController> _logger;
    private readonly IDocumentTemplateRepository _repository;

    public RequestResponseController(
        ILogger<RequestResponseController> logger,
        IDocumentTemplateRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost("documents/{documentId:guid}")]
    public async Task<IActionResult> TestMeAsync(Guid documentId, CancellationToken cancellationToken)
    {
        var documentTemplate = await _repository.GetById(documentId, cancellationToken);

        if (documentTemplate is null)
        {
            return NotFound();
        }

        return Ok(documentTemplate);
    }
}