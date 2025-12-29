using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Screening
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("screeningTime")]
    public required DateTime ScreeningTime { get; set; }
    [JsonPropertyName("pricePerSeat")]
    public required decimal PricePerSeat { get; set; }
    [JsonPropertyName("movieId")]
    public required int MovieId { get; set; }
}
