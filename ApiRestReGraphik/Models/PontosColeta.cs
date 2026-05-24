using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class PontosColeta
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("nome_ponto")]
        public required string NomePonto { get; set; }

        [JsonPropertyName("cidade")]
        public required string Cidade { get; set; }

        [JsonPropertyName("estado")]
        public required string Estado { get; set; }

        [JsonPropertyName("cep")]
        public required string CEP { get; set; } 

        [JsonPropertyName("residuos_aceitos")]
        public required string ResiduosAceitos { get; set; } 
    }
}
