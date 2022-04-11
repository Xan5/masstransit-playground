using System;

namespace Events;

public interface GetDocumentTemplateMetadataRequest
{
    Guid Id { get; }
}

public interface GetDocumentTemplateMetadataResposne
{
    Guid Id { get; }
    string Name { get; }
}

public interface GetDocumentLaterValueRequest
{
    Guid Id { get; }
}

public interface GetDocumentLaterValueResponse
{
    int LaterValue { get; }
}