using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class SugestaoResiduo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("id_cadastro_residuo")]
        [ForeignKey("CadastroResiduo")]
        public string IdCadastroResiduo { get; set; }

        /// <summary>
        /// Chamando a propriedade de navegação para o cadastro de resíduo associado
        /// </summary>

        [JsonIgnore]
        public virtual Residuo CadastroResiduo { get; set; }

        [JsonPropertyName("id_sugestao")]
        [ForeignKey("Sugestao")]
        public string IdSugestao { get; set; }

        /// <summary>
        /// Chamando a propriedade de navegação para a sugestão associada
        /// </summary>

        [JsonIgnore]
        public virtual Sugestao Sugestao { get; set; }

        [JsonPropertyName("data_aplicacao")]
        public DateTime? DataAplicacao { get; set; }
    }
}
