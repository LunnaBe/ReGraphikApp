using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class Residuo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("id_usuario")]
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }

        /// <summary>
        /// Propriedade de navegação para o usuário associado
        /// </summary>
        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }

        [JsonPropertyName("tipo_residuo")]
        public string TipoResiduo { get; set; }

        [JsonPropertyName("origem")]
        public string Origem { get; set; }

        [JsonPropertyName("especificacao")]
        public string Especificacao { get; set; }

        [JsonPropertyName("projeto")]
        public string Projeto { get; set; }

        [JsonPropertyName("quantidade")]
        public double Quantidade { get; set; }

        [JsonPropertyName("data_cadastro")]
        public DateTime DataCadastro { get; set; }

        [JsonPropertyName("condicao")]
        public string Condicao { get; set; }

        [JsonPropertyName("dimensoes_cm")]
        public double? DimensoesCm { get; set; }

        [JsonPropertyName("dimensoes_lm")]
        public double? DimensoesLm { get; set; }

        [JsonPropertyName("observacao")]
        public string Observacao { get; set; }

        [JsonPropertyName("anexo")]
        public string Anexo { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}