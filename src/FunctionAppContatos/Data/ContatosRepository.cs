using Azure.Data.Tables;
using Testcontainers.Azurite;

namespace FunctionAppContatos.Data;

public class ContatosRepository
{
    private TableClient _tableClient;

    public ContatosRepository(AzuriteContainer azuriteContainer)
    {
        _tableClient = new TableClient(
            azuriteContainer.GetConnectionString(), "Contatos");
    }
    
    public void Insert(ContatoEntity contato)
    {
        _tableClient.AddEntity(contato);
    }

    public IEnumerable<ContatoEntity> GetAll()
    {
        return _tableClient.Query<ContatoEntity>();
    }
}