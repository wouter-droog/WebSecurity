using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebSecurity.Models.Dtos;

namespace WebSecurity.Pages;

[Authorize(Policy = Constants.HRClaimPolicy)]
public class HumanResource : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    [BindProperty]
    public List<WeatherForecastDto> WeatherForecastDtos { get; set; }   

    public HumanResource(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task OnGetAsync()
    {
        var client = _httpClientFactory.CreateClient("OurWebAPI");
        WeatherForecastDtos = await client.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast") ?? new List<WeatherForecastDto>();
    }
}