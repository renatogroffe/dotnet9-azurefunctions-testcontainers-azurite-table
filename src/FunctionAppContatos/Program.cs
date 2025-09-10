using Azure.Data.Tables;
using FunctionAppContatos.Data;
using FunctionAppContatos.Utils;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.Azurite;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

Console.WriteLine("***** Iniciando testes com Testcontainers + Azure Table Storage *****");
CommandLineHelper.Execute("docker container ls",
    "Containers antes da execucao do Testcontainers...");

var azuriteContainer = new AzuriteBuilder()
  .WithImage("mcr.microsoft.com/azure-storage/azurite:3.34.0")
  .Build();
await azuriteContainer.StartAsync();

CommandLineHelper.Execute("docker container ls",
    "Containers apos execucao do Testcontainers...");

var connectionStringTableStorage = azuriteContainer.GetConnectionString();
const string tableName = "Contatos";

Console.WriteLine($"Connection String = {connectionStringTableStorage}");
Console.WriteLine($"Table Endpoint = {azuriteContainer.GetTableEndpoint()}");
Console.WriteLine($"Table a ser utilizada nos testes = {tableName}");

var tableClient = new TableClient(connectionStringTableStorage, tableName);
tableClient.CreateIfNotExists();
Console.WriteLine($"Table {tableName} criada com sucesso!");
Console.WriteLine();

builder.Services.AddSingleton(azuriteContainer);
builder.Services.AddScoped<ContatosRepository>();

builder.Build().Run();