using System.Text.Json.Serialization;

namespace ApiRestReGraphik.Models
{
    public class PontosColeta
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nome_ponto")]
        public string NomePonto { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("cep")]
        public string CEP { get; set; } 

        [JsonPropertyName("residuos_aceitos")]
        public string ResiduosAceitos { get; set; }

        [JsonPropertyName("latitude")]
        public double Lat { get; set; }

        [JsonPropertyName("longitude")]
        public double Lng { get; set; }
    }
}
