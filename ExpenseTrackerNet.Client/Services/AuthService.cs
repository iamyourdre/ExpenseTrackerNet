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
}
