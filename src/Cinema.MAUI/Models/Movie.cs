using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}
