using Bogus.DataSets;
using FunctionAppContatos.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionAppContatos;

public class Functions
{
    private readonly ILogger<Functions> _logger;
    private readonly ContatosRepository _repository;

    public Functions(ILogger<Functions> logger,
        ContatosRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [Function("Contatos")]
    public IActionResult RunGetContatos([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var contatos = _repository.GetAll();
        _logger.LogInformation($"Retornando {contatos.Count()} contatos...");
        return new OkObjectResult(contatos);
    }

    [Function("CreateContatoFake")]
    public IActionResult RunCreateContatoFake([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        var addressDataSet = new Address("pt_BR");
        var namesDataSet = new Name("pt_BR");
        var companyDataSet = new Company("pt_BR");
        var contato = new ContatoEntity
        {
            PartitionKey = addressDataSet.StateAbbr(),
            RowKey = Guid.NewGuid().ToString(),
            Nome = namesDataSet.FullName(),
            Empresa = companyDataSet.CompanyName()
        };
        _repository.Insert(contato);
        _logger.LogInformation(
            $"Inserindo novo contato {contato.PartitionKey} - {contato.RowKey} - {contato.Nome}");

        return new CreatedResult();
    }
}