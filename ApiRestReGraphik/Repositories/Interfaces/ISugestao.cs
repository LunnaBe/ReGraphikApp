using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados às sugestões, 
    /// como listar, obter por ID, adicionar, atualizar e excluir sugestões.
    /// </summary>
    public interface ISugestao
    {
        Task<List<Sugestao>> GetAll();
        Task<Sugestao> GetById(string id);
        Task Add(Sugestao sugestao);
        Task Update(string id, Sugestao sugestao);
        Task Delete(string id);
    }
}
