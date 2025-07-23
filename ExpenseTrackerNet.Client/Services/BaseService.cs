using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;

namespace ExpenseTrackerNet.Client.Services;

public abstract class BaseService
{
    protected readonly HttpClient _http;
    protected readonly IJSRuntime _js;
    protected readonly NavigationManager _nav;

    protected BaseService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
    {
        _http = factory.CreateClient("HttpClient");
        _js = js;
        _nav = nav;
    }
}