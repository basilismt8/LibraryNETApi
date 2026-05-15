using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanHistoryController : ControllerBase
    {
        private readonly ILoanHistoryService _loanHistoryService;
        private readonly ILogger<LoanHistoryController> _logger;

        public LoanHistoryController(ILoanHistoryService loanHistoryService, ILogger<LoanHistoryController> logger)
        {
            _loanHistoryService = loanHistoryService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/loanhistory called by {User}", User.Identity?.Name);
            var result = await _loanHistoryService.GetAllAsync(cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var result = await _loanHistoryService.GetByUserIdAsync(userId, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpGet("bookcopy/{bookCopyId:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetByBookCopy([FromRoute] Guid bookCopyId, CancellationToken cancellationToken)
        {
            var result = await _loanHistoryService.GetByBookCopyIdAsync(bookCopyId, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpGet("paired")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAllPaired(CancellationToken cancellationToken)
        {
            var result = await _loanHistoryService.GetAllPairedAsync(cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
