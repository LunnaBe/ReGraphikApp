using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class Usuario
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("name")]
        public required string Nome { get; set; }

        [JsonPropertyName("cpf")]
        public required string CPF { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("login")]
        public required string Login { get; set; }

        [JsonPropertyName("senha")]
        public required string Senha { get; set; }

        [JsonPropertyName("data_cadastro")]
        public DateTime DataCadastro { get; set; }
    }
}
