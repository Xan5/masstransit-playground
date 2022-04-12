using System;

namespace Events;

public interface StartTestSaga
{
    public Guid CorrelationId { get; }
    public Guid TemplateId { get; }
}
