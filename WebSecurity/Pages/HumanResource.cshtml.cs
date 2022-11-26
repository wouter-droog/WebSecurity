using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebSecurity.Authorization;
using WebSecurity.Models.Dtos;

namespace WebSecurity.Pages;

[Authorize(Policy = Constants.HRClaimPolicy)]
public class HumanResource : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    [BindProperty]
    public List<WeatherForecastDto>? WeatherForecastDtos { get; set; }   

    public HumanResource(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient("OurWebAPI");
    }
    
    public async Task OnGetAsync()
    {
        WeatherForecastDtos = await InvokeEndPoint<List<WeatherForecastDto>>("OurWebAPI", "WeatherForecast");
    }
    
    private async Task<T?> InvokeEndPoint<T>(string clientName, string url)
    {
        // get token from session
        JwtToken? token = null;

        var strTokenObj = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrWhiteSpace(strTokenObj))
            token = await Authenticate();
        else
            token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);

        if (token == null ||
            string.IsNullOrWhiteSpace(token.Token) ||
            token.ExpireDateTime <= DateTime.UtcNow)
            token = await Authenticate();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        return await _client.GetFromJsonAsync<T>(url);
    }


    private async Task<JwtToken> Authenticate()
    {
        var res = await _client.PostAsJsonAsync("auth", new Credential { UserName = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        var strJwt = await res.Content.ReadAsStringAsync();
        HttpContext.Session.SetString("access_token", strJwt);

        return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
    }
}