using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados aos pontos de coleta, 
    /// como listar, obter por ID, adicionar, atualizar e excluir pontos de coleta.
    /// </summary>
    public interface IPontosColeta
    {
        Task<List<PontosColeta>> GetAll();
        Task<PontosColeta> GetById(string id);
        Task Add(PontosColeta pontoColeta);
        Task Update(string id, PontosColeta pontoColeta);
        Task Delete(string id);
    }
}
