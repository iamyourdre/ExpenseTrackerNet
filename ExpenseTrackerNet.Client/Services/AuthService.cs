using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ExpenseTrackerNet.Client.Services;


public interface IAuthService
{
    Task<bool> IsAuthAsync();
}

public class AuthService : BaseService, IAuthService
{

    public AuthService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
        : base(factory, js, nav) { }

    public async Task<bool> IsAuthAsync()
    {
        var accessToken = await GetTokenAsync("localStorage", "accessToken");
        var refreshToken = await GetTokenAsync("sessionStorage", "refreshToken");

        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        if (IsTokenValid(accessToken))
        {
            return true;
        }

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
