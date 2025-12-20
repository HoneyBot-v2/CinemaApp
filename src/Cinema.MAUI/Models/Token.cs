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
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the token (e.g., "Bearer").
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Gets or sets the unique identifier of the authenticated user.
    /// </summary>
    [PreferenceKey("cinema.auth.user_id")]
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }

    /// <summary>
    /// Gets or sets the display name of the authenticated user.
    /// </summary>
    [JsonPropertyName("user_name")]
    [PreferenceKey("cinema.auth.user_name")]
    public string UserName { get; init; } = string.Empty;
}