using ExpenseTrackerNet.Server.Controllers;
using ExpenseTrackerNet.Shared.Models;
using ExpenseTrackerNet.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class TransactionControllerTests
{
    private readonly Mock<ITransactionService> _serviceMock;
    private readonly TransactionController _controller;
    private readonly Guid _userId;

    public TransactionControllerTests()
    {
        _serviceMock = new Mock<ITransactionService>();
        _userId = Guid.NewGuid();
        _controller = new TransactionController(_serviceMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateTransactionAsync_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = await _controller.CreateTransactionAsync(null);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateTransactionAsync_ReturnsOk_WhenCreated()
    {
        var dto = new TransactionWriteDTO { UserId = _userId, Description = "desc", Category = "cat", Amount = 10, Date = DateTime.Now, Type = "Income" };
        var readDto = new TransactionReadDTO { Id = Guid.NewGuid(), UserId = _userId, Description = "desc", Category = dto.Category, Amount = 10, Date = dto.Date, dto.Type = "Income" };
        _serviceMock.Setup(s => s.CreateTransactionAsync(It.IsAny<TransactionWriteDTO>())).ReturnsAsync(readDto);

        var result = await _controller.CreateTransactionAsync(dto);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(readDto, ok.Value);
    }

    [Fact]
    public async Task UpdateTransactionAsync_ReturnsBadRequest_WhenUpdateFails()
    {
        var dto = new TransactionUpdateDTO { Id = Guid.NewGuid(), UserId = _userId, Description = "desc", Category = "cat", Amount = 10, Date = DateTime.Now, Type = "Income" };
        _serviceMock.Setup(s => s.UpdateTransactionAsync(_userId, dto)).ReturnsAsync((TransactionReadDTO)null);

        var result = await _controller.UpdateTransactionAsync(dto);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ReturnsNotFound_WhenNotFound()
    {
        _serviceMock.Setup(s => s.GetTransactionByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TransactionReadDTO)null);

        var result = await _controller.GetTransactionByIdAsync(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetUserTransactionsAsync_ReturnsOk_WhenFound()
    {
        var list = new List<TransactionReadDTO> { new TransactionReadDTO { Id = Guid.NewGuid(), UserId = _userId, Description = "desc", Category = "cat", Amount = 10, Date = DateTime.Now, Type = "Income" } };
        _serviceMock.Setup(s => s.GetUserTransactionAsync(_userId)).ReturnsAsync(list);

        var result = await _controller.GetUserTransactionsAsync();
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ReturnsOk_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteTransactionAsync(_userId, It.IsAny<Guid>())).ReturnsAsync(true);

        var result = await _controller.DeleteTransactionAsync(Guid.NewGuid());
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteTransactionAsync_ReturnsNotFound_WhenNotDeleted()
    {
        _serviceMock.Setup(s => s.DeleteTransactionAsync(_userId, It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _controller.DeleteTransactionAsync(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }
}