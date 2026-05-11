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
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;
        private readonly ILogger<FinesController> _logger;

        public FinesController(IFineService fineService, ILogger<FinesController> logger)
        {
            _fineService = fineService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/fines called by {User}", User.Identity?.Name);
            var fines = await _fineService.GetAllAsync(cancellationToken);
            return Ok(fines);
        }

        [HttpGet("user/current")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> GetCurrentUserFines(CancellationToken cancellationToken)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized("User ID not found in token.");

            var userId = Guid.Parse(userIdStr);
            var fines = await _fineService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(fines);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _fineService.GetByIdAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost("addFine")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddFine([FromBody] AddFineRequestDto addFineRequestDto, CancellationToken cancellationToken)
        {
            var result = await _fineService.AddFineAsync(addFineRequestDto, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost("processOverdueLoans/{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ProcessOverdueLoans(Guid id, CancellationToken cancellationToken)
        {
            var result = await _fineService.ProcessOverdueLoansAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPatch("{id:Guid}/pay")]
        [Authorize(Roles = "Librarian,Member")]
        public async Task<IActionResult> PayFine([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _fineService.PayFineAsync(id, cancellationToken);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
