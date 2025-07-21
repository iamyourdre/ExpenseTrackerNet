using Azure.Core;
using ExpenseTrackerNet.Server.Models;
using ExpenseTrackerNet.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTrackerNet.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] TransactionWriteDTO request)
        {
            if (request == null)
            {
                return BadRequest("Transaction data is required.");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            request.UserId = userId;
            var result = await _transactionService.CreateTransactionAsync(request);
            if (result == null)
            {
                return BadRequest("Failed to create transaction.");
            }
            return Ok(result);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] TransactionUpdateDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Transaction data with a valid Id is required.");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            request.UserId = userId;
            var result = await _transactionService.UpdateTransactionAsync(request);
            if (result == null)
            {
                return BadRequest("Failed to update transaction.");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Transaction Id is required.");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            var result = await _transactionService.GetTransactionByIdAsync(id);
            if (result == null)
            {
                return NotFound("Transaction not found.");
            }
            if (result.UserId != userId)
            {
                return Forbid("You do not have permission to access this transaction.");
            }
            return Ok(result);
        }

        // GetUserTransactionAsync
        [HttpGet]
        public async Task<IActionResult> GetUserTransactionsAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            var result = await _transactionService.GetUserTransactionAsync(userId);
            if (result == null || !result.Any())
            {
                return NotFound("No transactions found for this user.");
            }
            return Ok(result);
        }
    }
}
