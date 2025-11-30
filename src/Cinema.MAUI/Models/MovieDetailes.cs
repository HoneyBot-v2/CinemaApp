using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class MovieDetailes
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    [JsonPropertyName("description")]
    public required string Description { get; set; }
    [JsonPropertyName("release_date")]
    public required DateTime ReleaseDate { get; set; }
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    [JsonPropertyName("duration")]
    // TODO: Change to TimeSpan
    public required string Duration { get; set; }
    [JsonPropertyName("image_url")]
    public required string ImageUrl { get; set; }
}
