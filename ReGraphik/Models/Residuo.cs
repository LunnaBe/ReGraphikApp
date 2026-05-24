using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class Residuo
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("id_usuario")]
        public required string IdUsuario { get; set; }

        [JsonPropertyName("tipo_residuo")]
        public required string TipoResiduo { get; set; }

        [JsonPropertyName("origem")]
        public required string Origem { get; set; }

        [JsonPropertyName("especificacao")]
        public required string Especificacao { get; set; }

        [JsonPropertyName("projeto")]
        public required string Projeto { get; set; }

        [JsonPropertyName("quantidade")]
        public double Quantidade { get; set; }

        [JsonPropertyName("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        [JsonPropertyName("condicao")]
        public required string Condicao { get; set; }

        [JsonPropertyName("dimensoes_cm")]
        public double? DimensoesCm { get; set; }

        [JsonPropertyName("dimensoes_lm")]
        public double? DimensoesLm { get; set; }

        [JsonPropertyName("observacao")]
        public required string Observacao { get; set; }

        [JsonPropertyName("anexo")]
        public required string Anexo { get; set; }

        [JsonPropertyName("status")]
        public required string Status { get; set; }
    }
}
