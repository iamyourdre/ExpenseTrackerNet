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
                return Unauthorized("UserId claim missing or invalid.");
            }

            request.UserId = userId;

            var result = await _transactionService.CreateTransactionAsync(request);
            if (result == null)
            {
                return BadRequest("Failed to create transaction.");
            }
            return Ok(result);
        }
    }
}
