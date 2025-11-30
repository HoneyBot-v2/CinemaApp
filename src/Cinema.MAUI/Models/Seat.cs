using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Seat
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("row")]
    public required string Row { get; set; }
    [JsonPropertyName("seat_number")]
    public required int SeatNumber { get; set; }
    [JsonPropertyName("is_available")]
    public required bool IsAvailable { get; set; }
}
