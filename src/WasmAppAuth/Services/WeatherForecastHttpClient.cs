using Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WasmAppAuth.Services
{
    public class WeatherForecastHttpClient
    {
        public HttpClient _httpClient;

        public WeatherForecastHttpClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<List<WeatherForecast>> GetWeatherForecastsAsync()
        {
            var response = await _httpClient.GetAsync("/weatherforecasts");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();
        }
    }
}
