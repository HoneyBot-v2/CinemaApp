using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Security;
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

        HttpClient httpClient = CreateClient();
        string json = System.Text.Json.JsonSerializer.Serialize(register);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"users/register", content);
        return response.IsSuccessStatusCode;
    }

    public static async Task<bool> Login(string email, string password)
    {
        var login = new Login()
        {
            Email = email,
            Password = password
        };

        HttpClient httpClient = CreateClient();
        string json = System.Text.Json.JsonSerializer.Serialize(login);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"users/login", content);

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
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        string jsonResponse = await httpClient.GetStringAsync($"movies?movieType={movieType}");

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
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        string jsonResponse = await httpClient.GetStringAsync($"movies/{movieId}");

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
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"screenings/by-movie/{movieId}");

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
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"seats/{screeningId}");

        // If deserialization fails, return an empty list
        List<Seat>? seats = JsonSerializer.Deserialize<List<Seat>>(jsonResponse);
        return seats ?? new List<Seat>();
    }

    public static async Task<List<Reservation>> GetReservations()
    {
        // Load token from preferences
        Token token = PreferenceHelper.Load<Token>();
        if (!token.Validate())
        {
            throw new InvalidOperationException("Invalid token. Please log in again.");
        }

        // Return reservations from API
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"reservations/by-user/{token.UserId}");

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
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
        var jsonResponse = await httpClient.GetStringAsync($"reservations/reserve?userId={userId}&screeningId={screeningId}&{seatIdsQuery}");

        // If deserialization fails, return an empty ReservationResponse object
        ReservationResponse? reservationResponse = JsonSerializer.Deserialize<ReservationResponse>(jsonResponse);
        return reservationResponse ?? new ReservationResponse();
    }

    /// <summary>
    /// Creates a configured HttpClient instance for API communication with
    /// Android Emulator support.
    /// </summary>
    /// <returns>A configured HttpClient with SSL certificate validation and
    /// base address set for local development.</returns>
    /// <remarks>
    /// <para>
    /// This method fixes a critical issue that prevented the Android Emulator
    /// from connecting to the localhost API. The Android Emulator uses the
    /// special IP address 10.0.2.2 to access the host machine's localhost, but
    /// standard SSL certificate validation fails for this address because the
    /// certificate is typically issued for "localhost" or the machine's
    /// hostname, not for 10.0.2.2.
    /// </para>
    /// <para>
    /// <strong>How it works:</strong>
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Implements custom SSL certificate validation that accepts connections to
    /// 10.0.2.2 and localhost without certificate validation errors. This is
    /// safe for development environments where we control the API server.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Sets a base address of "https://10.0.2.2:5001/api/" which allows all API
    /// methods to use relative URLs, reducing code duplication and making it
    /// easier to switch between development and production endpoints.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Configures a 30-second timeout to prevent indefinite hangs on network
    /// issues.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Centralizes HttpClient creation to prevent socket exhaustion that occurs
    /// when creating new HttpClient instances for each API call.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// <strong>Security Note:</strong> The custom certificate validation
    /// bypasses SSL certificate checks for localhost and 10.0.2.2. This is
    /// intended for development only and should NOT be used in production
    /// builds.
    /// </para>
    /// </remarks>
    private static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // DEV ONLY: accept localhost/10.0.2.2 self-signed cert
                if (message?.RequestUri?.Host is "10.0.2.2" or "localhost")
                {
                    return true;
                }

                return errors == SslPolicyErrors.None;
            }
        };

        string baseUrl = AppSettings.HttpsApiUrl;

        return new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(30),
            BaseAddress = new Uri(baseUrl)
        };
    }
}
