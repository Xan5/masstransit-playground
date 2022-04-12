using System;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shop;
using Shop.Sagas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(
    massTransit =>
    {
        // massTransit.UsingRabbitMq(
        //     (context, rabbitCfg) =>
        //     {
        //         rabbitCfg.Host(
        //             "127.0.0.1",
        //             "/",
        //             host =>
        //             {
        //                 host.Username("rabbituser");
        //                 host.Password("rabbitpassword");
        //             });
        //
        //         rabbitCfg.ConfigureEndpoints(context);
        //     });

        massTransit.UsingAzureServiceBus((context, azure) =>
        {
            azure.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));
            azure.ConfigureEndpoints(context);
        });

        massTransit.AddRequestClient<GetDocumentTemplateMetadataRequest>();
        massTransit.AddRequestClient<GetDocumentLaterValueRequest>();

        massTransit.AddSagaStateMachine<OrderPurchasingStateMachine, OrderPurchasingState>()
            .AzureTableRepository(sagaConfig =>
            {
                sagaConfig.ConnectionFactory(() => new CloudTable(
                    new Uri("https:/test"),
                    new StorageCredentials()));
            });

        massTransit.AddSagaStateMachine<TestStateMachine, TestState>()
            .AzureTableRepository(sagaConfig =>
            {
                sagaConfig.ConnectionFactory(() => new CloudTable(
                    new Uri("https:/test"),
                    new StorageCredentials()));
            });
    });

builder.Services.AddScoped<IDocumentTemplateRepository, FileStorageServiceBasedDocumentTemplateRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();