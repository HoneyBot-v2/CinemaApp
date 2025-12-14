using System;
using System.Collections.Generic;
using System.Text;
using Android.Hardware.Camera2.Params;
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
        HttpResponseMessage response = httpClient.PostAsync("https://localhost:5001/api/user/register", content).Result;

        return response.IsSuccessStatusCode;
    }
}
