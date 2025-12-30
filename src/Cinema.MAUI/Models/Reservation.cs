using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

public class Reservation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("reservationDate")]
    public required DateTime ReservationDate { get; set; }
    [JsonPropertyName("numberOfSeats")]
    public required int NumberOfSeats { get; set; }
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }
    [JsonPropertyName("userId")]
    public required int UserId { get; set; }
    [JsonPropertyName("screeningId")]
    public required int ScreeningId { get; set; }
    [JsonPropertyName("movieTitle")]
    public required string MovieTitle { get; set; }
    [JsonPropertyName("movieImageUrl")]
    public required string MovieImgUrl { get; set; }
    [JsonPropertyName("seatNumbers")]
    public required List<string> SeatNumbers { get; set; }
    public string SeatNumbersDisplay
    {
        get
        {
            return string.Join(",", SeatNumbers);
        }
    }
}
