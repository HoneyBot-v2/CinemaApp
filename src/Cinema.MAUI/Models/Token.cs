using System.Text.Json.Serialization;
using Cinema.MAUI.Attributes;

namespace Cinema.MAUI.Models;

internal class Token
{
    /// <summary>
    /// Gets or sets the OAuth access token used for authenticated requests.
    /// </summary>
    [JsonPropertyName("access_token")]
    [PreferenceKey("cinema.auth.access_token")]
    public required string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the type of the token (e.g., "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the authenticated user.
    /// </summary>
    [PreferenceKey("cinema.auth.user_id")]
    [JsonPropertyName("user_id")]
    public required int UserId { get; set; }

    /// <summary>
    /// Gets or sets the display name of the authenticated user.
    /// </summary>
    [JsonPropertyName("user_name")]
    [PreferenceKey("cinema.auth.user_name")]
    public required string UserName { get; set; }
}