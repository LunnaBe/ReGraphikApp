using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados ao ReGraphik, 
    /// como listar, obter por ID, adicionar, atualizar e excluir resíduos.
    /// </summary>
    public interface IReGraphikRepository
    {
        Task<List<Residuo>> GetAll();
        Task<Residuo> GetById(int id);
        Task Add(Residuo residuo);
        Task Update(int id, Residuo residuo);
        Task Delete(int id);
    }
}
