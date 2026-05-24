using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados ao ReGraphik, 
    /// como listar, obter por ID, adicionar, atualizar e excluir resíduos.
    /// </summary>
    public interface IResiduo
    {
        Task<List<Residuo>> GetAll();
        Task<Residuo> GetById(string id);
        Task Add(Residuo residuo);
        Task Update(string id, Residuo residuo);
        Task Delete(string id);
    }
}
