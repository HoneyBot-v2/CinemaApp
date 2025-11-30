using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Screening
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("screening_time")]
    public required DateTime ScreeningTime { get; set; }
    [JsonPropertyName("price_per_seat")]
    public required decimal PricePerSeat { get; set; }
    [JsonPropertyName("movie_id")]
    public required int MovieId { get; set; }
}
