using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class SugestaoResiduo
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("id_cadastro_residuo")]
        public int IdCadastroResiduo { get; set; }

        [JsonPropertyName("id_sugestao")]
        public int IdSugestao { get; set; }

        [JsonPropertyName("data_aplicacao")]
        public DateTime? DataAplicacao { get; set; }
    }
}
