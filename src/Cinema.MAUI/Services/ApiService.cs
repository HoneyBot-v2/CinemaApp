using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cinema.MAUI.Attributes;
using Cinema.MAUI.Helpers;
using Cinema.MAUI.Models;

namespace Cinema.MAUI.Services;

internal class ApiService
{
    public static async Task<bool> RegisterUser(string name, string email, string password)
    {
        Register register = new Register
        {
            Name = name,
            Email = email,
            Password = password
        };

        HttpClient httpClient = new HttpClient();
        string json = System.Text.Json.JsonSerializer.Serialize(register);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = httpClient.PostAsync($"{AppSettings.ApiUrl}/users/register", content).Result;

        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> Login(string email, string password)
    {
        var login = new Login()
        {
            Email = email,
            Password = password
        };

        HttpClient httpClient = new HttpClient();
        string json = System.Text.Json.JsonSerializer.Serialize(login);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"{AppSettings.ApiUrl}/users/login", content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        Token? token = JsonSerializer.Deserialize<Token>(jsonResponse);
        if (token is null)
        {
            return false;
        }

        // use the PreferenceHelper to save the token
        PreferenceHelper.Save(token);

        return true;
    }

    public static async Task<List<Movie>> GetMovies(string movieType)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();        
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }

        // Return movies from API with type
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        string jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/movies?movieType={movieType}");

        // If deserialization fails, return an empty list
        List<Movie>? movies = JsonSerializer.Deserialize<List<Movie>>(jsonResponse);
        return movies ?? new List<Movie>();
    }

    public static async Task<MovieDetailes> GetMovieDetails(int movieId)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }

        // Return movie details from API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        string jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/movies/{movieId}");

        // If deserialization fails, return an empty MovieDetailes object
        MovieDetailes? details = JsonSerializer.Deserialize<MovieDetailes>(jsonResponse);
        return details ?? new MovieDetailes();
    }

    public static async Task<List<Screening>> GetMovieScreening(int movieId)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }

        // Return movie screenings from API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/screanings/by-movie/{movieId}");

        // If deserialization fails, return an empty list
        List<Screening>? screenings = JsonSerializer.Deserialize<List<Screening>>(jsonResponse);
        return screenings ?? new List<Screening>();
    }

    public static async Task<List<Seat>> GetAllSeats(int screeningId)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }
        
        // Return seats from API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/seats/{screeningId}");
        
        // If deserialization fails, return an empty list
        List<Seat>? seats = JsonSerializer.Deserialize<List<Seat>>(jsonResponse);
        return seats ?? new List<Seat>();
    }

    public static async Task<List<Reservation>> GetReservationByUser(int userId)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }

        // Return reservations from API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/reservations/by-user/{userId}");
        
        // If deserialization fails, return an empty list
        List<Reservation>? reservations = JsonSerializer.Deserialize<List<Reservation>>(jsonResponse);
        return reservations ?? new List<Reservation>();
    }

    public static async Task<ReservationResponse> ReserveSeats(int userId, int screeningId, List<int> seatIds)
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (string.IsNullOrWhiteSpace(token.AccessToken))
        {
            throw new InvalidOperationException("Missing access token. Please log in.");
        }

        // TODO: Use a bitmask instead for more fun. ;)
        //
        // We list letters and numbers with letters for the row and the numbers
        // for the seat in that row can be a bit mask. Technically speaking this
        // allows for much more scalability and will be easier to implement
        // however if somebody reserves a seat in each row the string becomes
        // longer. This way we parse the "seatIds" by letters with a regex, get
        // the seat mask, then convert the mask to get the number of seats in
        // that row. 
        //
        // NOTE: doint it this way is actually a valid way for the API
        // controller to accept a list and is a good pattern when the list is
        // small. 
        //
        // Build seatIds query string as the tutorial currently does
        string seatIdsQuery = string.Join("&", seatIds.Select(id => $"seatIds={id}"));

        // Return reservation response from API
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"{AppSettings.ApiUrl}/reservations/reserve/userId={userId}&screeningId={screeningId}&{seatIdsQuery}");

        // If deserialization fails, return an empty ReservationResponse object
        ReservationResponse? reservationResponse = JsonSerializer.Deserialize<ReservationResponse>(jsonResponse);
        return reservationResponse ?? new ReservationResponse();
    }
}
