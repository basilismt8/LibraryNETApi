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
            var records = await _loanHistoryService.GetAllAsync(cancellationToken);
            return Ok(records);
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var records = await _loanHistoryService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(records);
        }

        [HttpGet("bookcopy/{bookCopyId:guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetByBookCopy([FromRoute] Guid bookCopyId, CancellationToken cancellationToken)
        {
            var records = await _loanHistoryService.GetByBookCopyIdAsync(bookCopyId, cancellationToken);
            return Ok(records);
        }
    }
}
