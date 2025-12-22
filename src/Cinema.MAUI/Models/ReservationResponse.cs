using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class ReservationResponse
{
    [JsonPropertyName("movie_id")]
    public int MovieId { get; set; }
    [JsonPropertyName("movie_name")]
    public string MovieName { get; set; } = string.Empty;
    [JsonPropertyName("screening_date")]
    public DateTime ScreeningDate { get; set; }
    [JsonPropertyName("seats")]
    public List<string> Seats { get; set; } = new List<string>();
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
