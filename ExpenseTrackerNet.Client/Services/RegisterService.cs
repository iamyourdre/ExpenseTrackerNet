using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTrackerNet.Client.Services;

public interface IRegisterService
{
    Task<RegisterResult> RegisterAsync(UserDTO RegisterModel);
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class RegisterService : BaseService, IRegisterService
{
    public RegisterService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
        : base(factory, js, nav) { }

    public async Task<RegisterResult> RegisterAsync(UserDTO RegisterModel)
    {
        var response = await _http.PostAsJsonAsync("api/auth/Register", RegisterModel);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new RegisterResult
            {
                Success = false,
                ErrorMessage = await response.Content.ReadAsStringAsync(),
                StatusCode = response.StatusCode
            };
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new RegisterResult
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = response.StatusCode
            };
        }

        if (!response.IsSuccessStatusCode)
        {
            return new RegisterResult
            {
                Success = false,
                ErrorMessage = "Unexpected error occurred.",
                StatusCode = response.StatusCode
            };
        }

        var tokenResult = await response.Content.ReadFromJsonAsync<TokenResponseDTO>();
        if (tokenResult == null)
        {
            return new RegisterResult
            {
                Success = false,
                ErrorMessage = "Invalid response from server.",
                StatusCode = response.StatusCode
            };
        }

        await _js.InvokeVoidAsync("localStorage.setItem", "accessToken", tokenResult.AccessToken);
        await _js.InvokeVoidAsync("sessionStorage.setItem", "refreshToken", tokenResult.RefreshToken);
        _nav.NavigateTo("/");
        return new RegisterResult
        {
            Success = true,
            StatusCode = response.StatusCode
        };
    }
}

