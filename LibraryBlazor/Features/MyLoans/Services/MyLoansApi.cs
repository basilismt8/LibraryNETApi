using LibraryBlazor.Features.MyLoans.Models;
using LibraryBlazor.Http;

namespace LibraryBlazor.Features.MyLoans.Services
{
    public class MyLoansApi
    {
        private readonly ApiClient _api;

        public MyLoansApi(ApiClient api)
        {
            _api = api;
        }

        public Task<ApiResult<IReadOnlyList<MyLoanDto>>> GetMyLoansAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<MyLoanDto>>("api/loans/getAllLoansByUserIdAsync", cancellationToken);
    }
}
