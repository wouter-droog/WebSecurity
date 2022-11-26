using Newtonsoft.Json;

namespace WebSecurity.Authorization;

public class JwtToken
{
    [JsonProperty("token")]
    public string? Token { get; set; } 
    
    [JsonProperty("expires")]
    public DateTime ExpireDateTime { get; set; }
}