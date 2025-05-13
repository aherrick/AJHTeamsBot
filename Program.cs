using AJHTeamsBot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// ai

builder.Services.AddSingleton(
    Kernel
        .CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: "gpt-4o",
            endpoint: builder.Configuration["AzureOpenAI:Endpoint"],
            apiKey: builder.Configuration["AzureOpenAI:ApiKey"]
        )
        .Build()
);

// Bot config
builder.Services.AddSingleton<BotFrameworkAuthentication>(sp =>
{
    return new ConfigurationBotFrameworkAuthentication(builder.Configuration.GetSection("Bot"));
});

builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBot, TeamsBot>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// ✅ Simple GET health check route
app.MapGet("/", () => Results.Ok("🤖 Bot app is up and running."));

app.MapPost(
    "/api/messages",
    async context =>
    {
        var adapter = context.RequestServices.GetRequiredService<CloudAdapter>();
        var bot = context.RequestServices.GetRequiredService<IBot>();
        var request = context.Request;
        var response = context.Response;

        await adapter.ProcessAsync(request, response, bot);
    }
);

app.Run();