using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ExpenseTrackerNet.Client.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(UserDTO loginModel);
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

    public async Task<bool> LoginAsync(UserDTO loginModel)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", loginModel);

        if (!response.IsSuccessStatusCode)
            return false;

        var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseDTO>();
        if (tokenResult == null)
            return false;

        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResult.AccessToken);
        await _js.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", tokenResult.RefreshToken);
        _nav.NavigateTo("/");
        return true;
    }
}
