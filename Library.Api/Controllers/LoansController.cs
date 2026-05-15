using Library.Api.CustomActionFilters;
using Library.Api.Models.Dto;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;

        public LoansController(ILoanService loanService, ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/loans called by {User}", User.Identity?.Name);
            var result = await _loanService.GetAllAsync(cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUserLoans(CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var result = await _loanService.GetByUserIdAsync(userId, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _loanService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> Create([FromBody] CreateLoanRequestDto createLoanRequestDto, CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var result = await _loanService.CreateAsync(userId, createLoanRequestDto, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPut("{id:Guid}/extend")]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> ExtendLoanPeriod([FromRoute] Guid id, [FromBody] ExtendLoanRequestDto extendLoanRequestDto, CancellationToken cancellationToken)
        {
            var result = await _loanService.ExtendAsync(id, extendLoanRequestDto, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
