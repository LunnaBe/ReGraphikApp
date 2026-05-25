using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class Sugestao
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("tipo_residuo_aceito")]
        public string TipoResiduoAceito { get; set; }

        [JsonPropertyName("descricao_sugestao")]
        public string DescricaoSugestao { get; set; }
    }
}
