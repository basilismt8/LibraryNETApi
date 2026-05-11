using LibraryBlazor.Features.MyLoans.Models;
using LibraryBlazor.Features.MyLoans.Services;
using LibraryBlazor.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Features.MyLoans.Pages;

public partial class MyLoanList : IDisposable
{
    [Inject] private MyLoansApi MyLoansApi { get; set; } = default!;

    private readonly CancellationTokenSource _cts = new();

    private List<MyLoanRowVm> Loans = new();
    private List<MyFineRowVm> Fines = new();
    private string _activeTab = "loans";
    private bool _showModal;
    private bool _saving;
    private string FilterId = "";
    private MyLoanRowVm? _selectedLoan;
    private string FilterTitle = "";
    private int CurrentPage = 1;
    private int PageSize = 10;
    private DateOnly _newDueDate;
    private string? _modalError;
    private string FinesFilterTitle = "";
    private int FinesCurrentPage = 1;
    private int FinesPageSize = 10;

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    private async Task ReloadAsync()
    {
        var result = await MyLoansApi.GetMyLoansAsync(_cts.Token);
        if (!result.Success)
        {
            Loans = new();
            _modalError = result.Message;
            return;
        }

        Loans = (result.Data ?? Array.Empty<MyLoanDto>())
            .Select(b => new MyLoanRowVm(b))
            .ToList();
    }

    private async Task LoadFinesAsync()
    {
        var finesResult = await MyLoansApi.GetMyFinesAsync(_cts.Token);
        Fines = finesResult.Success
            ? (finesResult.Data ?? Array.Empty<MyFineDto>()).Select(f => new MyFineRowVm(f)).ToList()
            : new();
    }

    private async Task SelectFinesTab()
    {
        _activeTab = "fines";
        await LoadFinesAsync();
    }

    private async Task ReturnBookAsync(MyLoanRowVm loan)
    {
        var result = await MyLoansApi.ReturnBookAsync(loan.UserId, loan.BookCopyId, _cts.Token);
        if (result.Success)
            await ReloadAsync();
    }

    private async Task PayFineAsync(MyFineRowVm fine)
    {
        var result = await MyLoansApi.PayFineAsync(fine.Id, _cts.Token);
        if (result.Success)
        {
            await LoadFinesAsync();
            await ReloadAsync();
        }
    }

    private Task OpenExtendLoan(MyLoanRowVm loan)
    {
        _selectedLoan = loan;
        _newDueDate = loan.DueDate;
        _modalError = null;
        _showModal = true;
        return Task.CompletedTask;
    }

    private void CloseModal()
    {
        if (_saving) return;
        _showModal = false;
        _modalError = null;
    }

    private async Task SaveAsync()
    {
        if (_selectedLoan is null) return;

        _saving = true;
        _modalError = null;

        try
        {
            if (string.IsNullOrWhiteSpace(_newDueDate.ToString()))
            {
                _modalError = "DueDate is required.";
                return;
            }

            var result = await MyLoansApi.ExtendLoanAsync(_selectedLoan.Id, new ExtendLoanRequestDto(_newDueDate), _cts.Token);

            if (!result.Success)
            {
                if (result.ValidationErrors != null && result.ValidationErrors.Any())
                    _modalError = string.Join("\n", result.ValidationErrors.SelectMany(e => e.Value));
                else
                    _modalError = result.Message ?? "Unknown error";

                return;
            }

            _showModal = false;
            await ReloadAsync();
        }
        finally
        {
            _saving = false;
            StateHasChanged();
        }
    }

    public void Dispose() => _cts.Cancel();
}
