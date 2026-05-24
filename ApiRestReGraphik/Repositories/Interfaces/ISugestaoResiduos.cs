using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados às sugestões de resíduos, 
    /// como listar, obter por ID, adicionar, atualizar e excluir sugestões de resíduos.
    /// </summary>
    public interface ISugestaoResiduos
    {
        Task<List<SugestaoResiduo>> GetAll();
        Task<SugestaoResiduo> GetById(string id);
        Task Add(SugestaoResiduo sugestaoResiduo);
        Task Update(string id, SugestaoResiduo sugestaoResiduo);
        Task Delete(string id);
    }
}
