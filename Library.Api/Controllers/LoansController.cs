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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/loans called by {User}", User.Identity?.Name);
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUserLoans()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var loans = await _loanService.GetByUserIdAsync(userId);
            return Ok(loans);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _loanService.GetByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> Create([FromBody] CreateLoanRequestDto createLoanRequestDto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var result = await _loanService.CreateAsync(userId, createLoanRequestDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPut("{id:Guid}/extend")]
        [validateModel]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> ExtendLoanPeriod([FromRoute] Guid id, [FromBody] ExtendLoanRequestDto extendLoanRequestDto)
        {
            var result = await _loanService.ExtendAsync(id, extendLoanRequestDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
