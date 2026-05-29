using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class SugestaoResiduo
    {
        [JsonIgnore]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("id_cadastro_residuo")]
        [ForeignKey("CadastroResiduo")]
        public string? IdCadastroResiduo { get; set; }

        /// <summary>
        /// Chamando a propriedade de navegação para o cadastro de resíduo associado
        /// </summary>

        [JsonIgnore]
        [ValidateNever]
        public virtual Residuo? CadastroResiduo { get; set; }

        [JsonPropertyName("id_sugestao")]
        [ForeignKey("Sugestao")]
        public string? IdSugestao { get; set; }

        /// <summary>
        /// Chamando a propriedade de navegação para a sugestão associada
        /// </summary>

        [JsonIgnore]
        [ValidateNever]
        public virtual Sugestao? Sugestao { get; set; }

        [JsonPropertyName("data_aplicacao")]
        public DateTime? DataAplicacao { get; set; }
    }
}
