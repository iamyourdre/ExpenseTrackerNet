using ExpenseTrackerNet.Server.Controllers;
using ExpenseTrackerNet.Shared.Models;
using ExpenseTrackerNet.Server.Services;
using ExpenseTrackerNetApp.ApiService.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading.Tasks;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = await _controller.Register(null);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenUserIsCreated()
    {
        var userDto = new UserDTO { Username = "testuser", Password = "password123" };
        var user = new User { Id = System.Guid.NewGuid(), Username = "testuser" };
        _authServiceMock.Setup(s => s.RegisterAsync(userDto)).ReturnsAsync(user);

        var result = await _controller.Register(userDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenLoginFails()
    {
        var userDto = new UserDTO { Username = "testuser", Password = "wrongpass" };
        _authServiceMock.Setup(s => s.LoginAsync(userDto)).ReturnsAsync((TokenResponseDTO)null);

        var result = await _controller.Login(userDto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenLoginSucceeds()
    {
        var userDto = new UserDTO { Username = "testuser", Password = "password123" };
        var tokenResponse = new TokenResponseDTO { AccessToken = "token", RefreshToken = "refresh" };
        _authServiceMock.Setup(s => s.LoginAsync(userDto)).ReturnsAsync(tokenResponse);

        var result = await _controller.Login(userDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tokenResponse, okResult.Value);
    }

    [Fact]
    public async Task RefreshToken_ReturnsUnauthorized_WhenRefreshFails()
    {
        var refreshDto = new RefreshTokenRequestDTO { UserId = System.Guid.NewGuid(), RefreshToken = "badtoken" };
        _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto)).ReturnsAsync((TokenResponseDTO)null);

        var result = await _controller.RefreshToken(refreshDto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenRefreshSucceeds()
    {
        var refreshDto = new RefreshTokenRequestDTO { UserId = System.Guid.NewGuid(), RefreshToken = "goodtoken" };
        var tokenResponse = new TokenResponseDTO { AccessToken = "token", RefreshToken = "refresh" };
        _authServiceMock.Setup(s => s.RefreshTokenAsync(refreshDto)).ReturnsAsync(tokenResponse);

        var result = await _controller.RefreshToken(refreshDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(tokenResponse, okResult.Value);
    }
}