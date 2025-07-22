using ExpenseTrackerNet.Client;
using ExpenseTrackerNet.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBootstrapBlazor();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:7116/")
});

builder.Services.AddHttpClient("HttpClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:7116/");
});

await builder.Build().RunAsync();
