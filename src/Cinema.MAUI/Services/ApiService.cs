using System;
using System.Collections.Generic;
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
}
