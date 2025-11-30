using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Cinema.MAUI.Models;

internal class Reservation
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("reservation_date")]
    public required DateTime ReservationDate { get; set; }
    [JsonPropertyName("number_of_seats")]
    public required int NumberOfSeats { get; set; }
    [JsonPropertyName("amount")]
    public required decimal Amount { get; set; }
    [JsonPropertyName("user_id")]
    public required int UserId { get; set; }
    [JsonPropertyName("screening_id")]
    public required int ScreeningId { get; set; }
    [JsonPropertyName("movie_title")]
    public required string MovieTitle { get; set; }
    [JsonPropertyName("movie_img_url")]
    public required string MovieImgUrl { get; set; }
    [JsonPropertyName("seat_numbers")]
    public required List<string> SeatNumbers { get; set; }
}
