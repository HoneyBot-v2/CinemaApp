using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class ReservationResponse
{
    [JsonPropertyName("movieId")]
    public int MovieId { get; set; }
    [JsonPropertyName("movieName")]
    public string MovieName { get; set; } = string.Empty;
    [JsonPropertyName("screeningDate")]
    public DateTime ScreeningDate { get; set; }
    [JsonPropertyName("seats")]
    public List<string> Seats { get; set; } = new List<string>();
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
