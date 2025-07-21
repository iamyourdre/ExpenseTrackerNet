using ExpenseTrackerNet.Server.Controllers;
using ExpenseTrackerNet.Server.Models;
using ExpenseTrackerNet.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _serviceMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _serviceMock = new Mock<ICategoryService>();
        _controller = new CategoryController(_serviceMock.Object);

        // Simulate authenticated user
        var userId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task CreateCategoryAsync_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = await _controller.CreateCategoryAsync(null);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_ReturnsOk_WhenCategoryCreated()
    {
        var dto = new CategoryWriteDTO { UserId = Guid.NewGuid(), Name = "Test", Icon = "icon" };
        var readDto = new CategoryReadDTO { Id = Guid.NewGuid(), Name = "Test", Icon = "icon" };
        _serviceMock.Setup(s => s.CreateCategoryAsync(It.IsAny<CategoryWriteDTO>())).ReturnsAsync(readDto);

        var result = await _controller.CreateCategoryAsync(dto);
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(readDto, ok.Value);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ReturnsNotFound_WhenUpdateFails()
    {
        var dto = new CategoryUpdateDTO { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Name = "Test", Icon = "icon" };
        _serviceMock.Setup(s => s.UpdateCategoryAsync(It.IsAny<Guid>(), dto)).ReturnsAsync((CategoryReadDTO)null);

        var result = await _controller.UpdateCategoryAsync(dto);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ReturnsNotFound_WhenNotFound()
    {
        _serviceMock.Setup(s => s.GetCategoryByIdAsync(It.IsAny<Guid>())).ReturnsAsync((CategoryReadDTO)null);

        var result = await _controller.GetCategoryByIdAsync(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetUserCategoriesAsync_ReturnsOk()
    {
        var list = new List<CategoryReadDTO> { new CategoryReadDTO { Id = Guid.NewGuid(), Name = "Test", Icon = "icon" } };
        _serviceMock.Setup(s => s.GetUserCategoriesAsync(It.IsAny<Guid>())).ReturnsAsync(list);

        var result = await _controller.GetUserCategoriesAsync();
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(list, ok.Value);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsNoContent_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(true);

        var result = await _controller.DeleteCategoryAsync(Guid.NewGuid());
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ReturnsNotFound_WhenNotDeleted()
    {
        _serviceMock.Setup(s => s.DeleteCategoryAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _controller.DeleteCategoryAsync(Guid.NewGuid());
        Assert.IsType<NotFoundObjectResult>(result);
    }
}