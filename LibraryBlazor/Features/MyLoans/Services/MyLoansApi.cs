using LibraryBlazor.Features.MyLoans.Models;
using LibraryBlazor.Http;
using LibraryBlazor.Shared.Models;

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
        => _api.GetResultAsync<IReadOnlyList<MyLoanDto>>("api/loans/user/current", cancellationToken);

        public Task<ApiResult<IReadOnlyList<MyFineDto>>> GetMyFinesAsync(CancellationToken cancellationToken = default)
        => _api.GetResultAsync<IReadOnlyList<MyFineDto>>("api/fines/user/current", cancellationToken);

        public Task<ApiResult> ExtendLoanAsync(Guid loanId, ExtendLoanRequestDto body, CancellationToken cancellationToken = default)
            => _api.PutAsync($"api/loans/{loanId}/extend", new
            {
                dueDate = body.DueDate
            }, cancellationToken);

        public Task<ApiResult> ReturnBookAsync(Guid userId, Guid bookCopyId, CancellationToken cancellationToken = default)
            => _api.PutAsync("api/books/returnBook", new
            {
                userId,
                bookCopyId
            }, cancellationToken);

        public Task<ApiResult> PayFineAsync(Guid fineId, CancellationToken cancellationToken = default)
            => _api.PatchAsync($"api/fines/{fineId}/pay", cancellationToken);
    }
}
