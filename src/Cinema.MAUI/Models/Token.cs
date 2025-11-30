using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Token
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    [JsonPropertyName("user_id")]
    public required int UserId { get; set; }
    [JsonPropertyName("user_name")]
    public required string UserName { get; set; }
}