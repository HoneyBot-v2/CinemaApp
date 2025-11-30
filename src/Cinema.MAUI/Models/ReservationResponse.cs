using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class ReservationResponse
{
    [JsonPropertyName("movie_id")]
    public required int MovieId { get; set; }
    [JsonPropertyName("movie_name")]
    public required string MovieName { get; set; }
    [JsonPropertyName("screening_date")]
    public required DateTime ScreeningDate { get; set; }
    [JsonPropertyName("seats")]
    public required List<string> Seats { get; set; }
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }
}
