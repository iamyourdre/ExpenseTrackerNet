using ExpenseTrackerNet.Client.Services;
using Microsoft.AspNetCore.Components;

public class AuthGuardComponent : ComponentBase
{
    [Inject] protected IAuthService AuthService { get; set; }
    [Inject] protected NavigationManager NavigationManager { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!await AuthService.IsAuthAsync())
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}