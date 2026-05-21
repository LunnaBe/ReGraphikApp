using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class TipoResiduo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao_residuo")]
        public required string DescricaoResiduo { get; set; }
    }
}
