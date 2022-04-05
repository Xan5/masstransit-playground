using Bank.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(
    massTransit =>
    {
        massTransit.UsingRabbitMq(
            (context, rabbitCfg) =>
            {
                rabbitCfg.Host(
                    "127.0.0.1",
                    "/",
                    host =>
                    {
                        host.Username("rabbituser");
                        host.Password("rabbitpassword");
                    });

                rabbitCfg.ConfigureEndpoints(context);
            });

        massTransit.AddConsumer<AmountProvisioningRequestedConsumer>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();