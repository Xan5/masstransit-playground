using Bank.Consumers;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

const string applicationName = "bank-application";

builder.Services.AddMassTransit(
    massTransit =>
    {
        massTransit.SetKebabCaseEndpointNameFormatter();
        massTransit.AddConsumers(typeof(ConsumersMarker).Assembly);
        massTransit.UsingAzureServiceBus((context, azure) =>
        {
            azure.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));

            azure.SubscriptionEndpoint<GetDocumentLaterValueRequest>(
                applicationName,
                endpoint =>
                {
                    endpoint.ConfigureConsumer<GetDocumentLaterValueRequestConsumer>(context);
                    endpoint.UseInMemoryOutbox();
                });

            azure.SubscriptionEndpoint<GetDocumentTemplateMetadataRequest>(
                applicationName,
                endpoint =>
                {
                    endpoint.ConfigureConsumer<GetDocumentTemplateMetadataRequestConsumer>(context);
                    endpoint.UseInMemoryOutbox();
                });

            // azure.ConfigureEndpoints(context);
        });

    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();