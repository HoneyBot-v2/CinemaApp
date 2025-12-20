using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class MovieDetailes
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    [JsonPropertyName("duration")]
    // TODO: Change to TimeSpan?
    public string Duration { get; set; } = string.Empty;
    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = string.Empty;
}
