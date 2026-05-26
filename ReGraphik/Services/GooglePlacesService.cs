using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json; 
using System.Threading.Tasks;
using ApiRestReGraphik.Models; 

namespace ReGraphik.Services
{
    public class GooglePlacesService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        
        private readonly string _apiKey = "AIzaSyCPeDl0hmzFeROHcUxPbnQUvAhOA_N-ros";

        public async Task<List<PontosColeta>> BuscarPostosNoBrasilAsync(string cidade, string material)
        {
            var listaDePostos = new List<PontosColeta>();

            try
            {
                ///Monta a string de busca de forma básica
                string termoBusca = $"{material} em {cidade}";
                string url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(termoBusca)}&key={_apiKey}";

                /// Faz o download do JSON online do Google
                string jsonResposta = await _httpClient.GetStringAsync(url);

                ///Lê o JSON nativamente usando o JsonDocument 
                using (JsonDocument doc = JsonDocument.Parse(jsonResposta))
                {
                    JsonElement root = doc.RootElement;

                    /// Verifica se a propriedade "results" existe no retorno do Google
                    if (root.TryGetProperty("results", out JsonElement resultados))
                    {
                        int idContador = 1;

                        /// Varre os resultados usando o foreach convencional
                        foreach (JsonElement item in resultados.EnumerateArray())
                        {
                            /// Cria o objeto usando a estrutura exata da sua classe PontosColeta.cs
                            var novoPonto = new PontosColeta
                            {
                                Id = idContador.ToString(),
                                NomePonto = item.GetProperty("name").GetString() ?? "Sem nome",
                                Cidade = cidade,
                                Estado = "UF",
                                CEP = "00000-000",
                                ResiduosAceitos = material
                            };

                            /// Se quiser mapear o endereço completo que vem do Google dentro do campo CEP ou Cidade temporariamente para testar:
                            if (item.TryGetProperty("formatted_address", out JsonElement enderecoElement))
                            {
                                novoPonto.Cidade = enderecoElement.GetString() ?? cidade;
                            }

                            listaDePostos.Add(novoPonto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                /// Se der erro de rede, exibe no painel de Output e não quebra o app 
                System.Diagnostics.Debug.WriteLine("Erro de conexão: " + ex.Message);
            }

            return listaDePostos;
        }
    }
}