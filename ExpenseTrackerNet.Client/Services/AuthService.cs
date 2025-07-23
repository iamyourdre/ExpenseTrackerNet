using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Net;

namespace ExpenseTrackerNet.Client.Services;

public class LoginResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public interface IAuthService
{
    Task<LoginResult> LoginAsync(UserDTO loginModel);
    Task<bool> IsAuthAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;

    public AuthService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
    {
        _http = factory.CreateClient("HttpClient");
        _js = js;
        _nav = nav;
    }

    public async Task<LoginResult> LoginAsync(UserDTO loginModel)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", loginModel);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = await response.Content.ReadAsStringAsync(),
                StatusCode = response.StatusCode
            };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new LoginResult
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = response.StatusCode
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Unexpected error occurred.",
                StatusCode = response.StatusCode
            };
        }

        var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseDTO>();
        if (tokenResult == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Invalid response from server.",
                StatusCode = response.StatusCode
            };
        }

        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResult.AccessToken);
        await _js.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", tokenResult.RefreshToken);
        _nav.NavigateTo("/");
        return new LoginResult
        {
            Success = true,
            StatusCode = response.StatusCode
        };
    }

    public async Task<bool> IsAuthAsync()
    {
        Console.WriteLine(">>>>>>>>> Checking authentication status...");
        var accessToken = await GetTokenAsync("localStorage", "accessToken");
        var refreshToken = await GetTokenAsync("sessionStorage", "refreshToken");

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            Console.WriteLine(">>>>>>>>> No access or refresh token found.");
            return false;
        }

        if (IsTokenValid(accessToken))
        {
            Console.WriteLine(">>>>>>>>> Access token is valid.");
            return true;
        }

        Console.WriteLine(">>>>>>>>> Access token is invalid or expired. Attempting to refresh...");
        return await TryRefreshTokenAsync(refreshToken);
    }

    private async Task<string?> GetTokenAsync(string storageType, string key)
    {
        var storage = storageType == "localStorage" ? "localStorage" : "sessionStorage";
        return await _js.InvokeAsync<string>($"{storage}.getItem", key);
    }

    private bool IsTokenValid(string token)
    {
        var exp = GetTokenExpiration(token);
        if (exp == null)
            return false;

        return exp > DateTimeOffset.UtcNow;
    }

    private DateTimeOffset? GetTokenExpiration(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        try
        {
            var payload = parts[1];
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            var jsonBytes = Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/'));
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            var exp = System.Text.Json.JsonDocument.Parse(json).RootElement.GetProperty("exp").GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(exp);
        }
        catch
        {
            return null;
        }
    }

    private async Task<bool> TryRefreshTokenAsync(string refreshToken)
    {
        Console.WriteLine(">>>>>>>>> Refreshing token...");
        try
        {
            var refreshRequest = new RefreshTokenRequestDTO { RefreshToken = refreshToken };
            var response = await _http.PostAsJsonAsync("api/auth/refresh-token", refreshRequest);

            if (!response.IsSuccessStatusCode)
                return false;

            var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseDTO>();
            if (tokenResult == null)
                return false;

            await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResult.AccessToken);
            await _js.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", tokenResult.RefreshToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
