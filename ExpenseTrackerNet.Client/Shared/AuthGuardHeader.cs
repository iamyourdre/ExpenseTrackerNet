using ExpenseTrackerNet.Client.Services;
using Microsoft.AspNetCore.Components;

public class AuthGuardHeader : ComponentBase
{
    [Inject] protected IAuthService AuthService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }

    protected async Task<bool> EnsureAuthenticatedAsync()
    {
        var isAuthenticated = await AuthService.IsAuthAsync();
        if (!isAuthenticated)
        {
            NavigationManager.NavigateTo("/login", true);
            return false;
        }
        return true;
    }
}