using ApiRestReGraphik.Models;

namespace ApiRestReGraphik.Repositories.Interface
{
    /// <summary>
    /// Define os métodos para acessar e manipular os dados relacionados ao ReGraphik, 
    /// como listar, obter por ID, adicionar, atualizar e excluir usuários.
    /// </summary>
    public interface IUsuario
    {
        Task<List<Usuario>> GetAll();
        Task<Usuario> GetById(string id);
        Task Add(Usuario usuario);
        Task Update(string id, Usuario usuario);
        Task Delete(string id);
    }
}
