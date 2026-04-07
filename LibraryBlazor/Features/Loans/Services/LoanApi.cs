using LibraryBlazor.Features.Books.Models;
using LibraryBlazor.Features.Loans.Models;
using LibraryBlazor.Http;

namespace LibraryBlazor.Features.Loans.Services
{
    public class LoanApi
    {
        private readonly ApiClient _api;

        public LoanApi(ApiClient api)
        {
            _api = api;
        }

        public Task<ApiResult<IReadOnlyList<LoanDto>>> GetLoansAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<LoanDto>>("api/loans/getAll", cancellationToken);
    }
}
