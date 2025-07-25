using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTrackerNet.Client.Services;

public interface ITransactionService
{
    Task<List<TransactionReadDTO>?> GetUserTransactionsAsync();
    Task<TransactionReadDTO> CreateTransactionAsync(TransactionWriteDTO transaction);
}

public class TransactionService : BaseService, ITransactionService
{
    public TransactionService(IHttpClientFactory factory, IJSRuntime js, NavigationManager nav)
        : base(factory, js, nav) { }

    public async Task<List<TransactionReadDTO>?> GetUserTransactionsAsync()
    {
        var accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.GetAsync("api/Transaction");
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return null;

        if (!response.IsSuccessStatusCode)
            return null;

        var transactions = await response.Content.ReadFromJsonAsync<List<TransactionReadDTO>>();
        return transactions;
    }

    public async Task<TransactionReadDTO?> CreateTransactionAsync(TransactionWriteDTO transaction)
    {
        var accessToken = await _js.InvokeAsync<string>("localStorage.getItem", "accessToken");

        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _http.PostAsJsonAsync("api/Transaction/create", transaction);
        if (!response.IsSuccessStatusCode)
            return null;

        var createdTransaction = await response.Content.ReadFromJsonAsync<TransactionReadDTO>();
        return createdTransaction;
    }
}