using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTrackerNet.Client.Services;

public interface ILoginService
{
    Task<LoginResult> LoginAsync(UserDTO loginModel);
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class LoginService : BaseService, ILoginService
{
    public LoginService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
        : base(factory, js, nav) { }

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

