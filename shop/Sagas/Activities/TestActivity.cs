using System;
using System.Threading.Tasks;
using Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Shop.Sagas.Activities;

public class TestActivity : IStateMachineActivity<TestState, StartTestSaga>
{
    private readonly IDocumentTemplateRepository _documentTemplateRepository;
    private readonly ILogger<TestActivity> _logger;

    public TestActivity(IDocumentTemplateRepository documentTemplateRepository, ILogger<TestActivity> logger)
    {
        _documentTemplateRepository = documentTemplateRepository;
        _logger = logger;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(GetOrderAmount));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(
        BehaviorContext<TestState, StartTestSaga> context,
        IBehavior<TestState, StartTestSaga> next)
    {
        var document = await _documentTemplateRepository.GetById(context.Message.TemplateId);
        _logger.LogInformation(
            "Document id:{id}, latervalue:{value}, name:{name}",
            document.Id,
            document.LaterValue,
            document.Name);
        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<TestState, StartTestSaga, TException> context,
        IBehavior<TestState, StartTestSaga> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }
}
