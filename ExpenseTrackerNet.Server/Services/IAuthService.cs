using ExpenseTrackerNet.Server.Models;
using ExpenseTrackerNetApp.ApiService.Entities;

namespace ExpenseTrackerNet.Server.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDTO request);
        Task<TokenResponseDTO?> LoginAsync(UserDTO request);
        Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
    }
}
