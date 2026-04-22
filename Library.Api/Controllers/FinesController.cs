using Library.Api.CustomActionFilters;
using Library.Api.Models.Dto;
using Library.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET /api/fines called by {User}", User.Identity?.Name);
            var fines = await _fineService.GetAllAsync();
            return Ok(fines);
        }

        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _fineService.GetByIdAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost("addFine")]
        [validateModel]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> AddFine([FromBody] AddFineRequestDto addFineRequestDto)
        {
            var result = await _fineService.AddFineAsync(addFineRequestDto);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }

        [HttpPost("processOverdueLoans/{id:Guid}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> ProcessOverdueLoans(Guid id)
        {
            var result = await _fineService.ProcessOverdueLoansAsync(id);
            if (!result.Success)
                return StatusCode(result.StatusCode, result.Error);
            return Ok(result.Data);
        }
    }
}
