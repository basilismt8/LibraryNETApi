using LibraryBlazor.Features.Books.Models;
using LibraryBlazor.Features.Loans.Models;
using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Features.Loans.Components;

public partial class LoansTable
{
    [Parameter, EditorRequired] public List<LoanRowVm> Items { get; set; } = new();

    [Parameter] public string FilterId { get; set; } = "";
    [Parameter] public EventCallback<string> FilterIdChanged { get; set; }

    [Parameter] public string FilterTitle { get; set; } = "";
    [Parameter] public EventCallback<string> FilterTitleChanged { get; set; }

    [Parameter] public string FilterBookCopyId { get; set; } = "";
    [Parameter] public EventCallback<string> FilterBookCopyIdChanged { get; set; }

    [Parameter] public int CurrentPage { get; set; } = 1;
    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    [Parameter] public EventCallback<LoanRowVm> OnExtendLoan { get; set; }

    //[Parameter] public EventCallback OnNew { get; set; }
    //[Parameter] public EventCallback OnEdit { get; set; }
    //[Parameter] public EventCallback OnDelete { get; set; }


    private IEnumerable<LoanRowVm> FilteredItems =>
       Items.Where(l =>
           (string.IsNullOrWhiteSpace(FilterId) || l.Id.ToString().Contains(FilterId, StringComparison.OrdinalIgnoreCase)) &&
           (string.IsNullOrWhiteSpace(FilterBookCopyId) || l.BookCopyId.ToString().Contains(FilterBookCopyId, StringComparison.OrdinalIgnoreCase)) &&
           (string.IsNullOrWhiteSpace(FilterTitle) || l.BookTitle.Contains(FilterTitle, StringComparison.OrdinalIgnoreCase)));

    private IEnumerable<LoanRowVm> PagedItems =>
        FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));

    private bool SelectAllRows
    {
        get => FilteredItems.Any() && FilteredItems.All(b => b.IsSelected);
        set
        {
            foreach (var b in FilteredItems)
                b.IsSelected = value;
        }
    }

    private static string FormatStatus(LoanStatus status) => status switch
    {
        LoanStatus.borrowed => "Borrowed",
        LoanStatus.returned => "Returned",
        LoanStatus.overdue  => "Overdue",
        _                   => status.ToString()
    };

    private static string GetStatusBadgeClass(LoanStatus status) => status switch
    {
        LoanStatus.borrowed => "badge-borrowed",
        LoanStatus.returned => "badge-returned",
        LoanStatus.overdue  => "badge-overdue",
        _                   => ""
    };

    private static string GetUrgencyClass(LoanRowVm loan)
    {
        if (loan.Status == LoanStatus.returned)
            return "";

        var daysLeft = loan.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber;
        return daysLeft <= 2 ? "table-danger"
             : daysLeft <= 7 ? "table-warning"
             : "";
    }

    private string GetRowClass(LoanRowVm loan)
    {
        var urgency = GetUrgencyClass(loan);
        var selected = loan.IsSelected ? "row-selected" : "";
        return $"{selected} {urgency}".Trim();
    }

    private void ToggleSelection(LoanRowVm loan) => loan.IsSelected = !loan.IsSelected;

    private async Task OnFilterIdInput(ChangeEventArgs e)
    {
        await FilterIdChanged.InvokeAsync(e.Value?.ToString() ?? "");
        await CurrentPageChanged.InvokeAsync(1);
    }

    private async Task OnFilterTitleInput(ChangeEventArgs e)
    {
        await FilterTitleChanged.InvokeAsync(e.Value?.ToString() ?? "");
        await CurrentPageChanged.InvokeAsync(1);
    }

    private async Task OnFilterBookCopyIdInput(ChangeEventArgs e)
    {
        await FilterBookCopyIdChanged.InvokeAsync(e.Value?.ToString() ?? "");
        await CurrentPageChanged.InvokeAsync(1);
    }

    private async Task FirstPage()
    {
        if (CurrentPage > 1)
            await CurrentPageChanged.InvokeAsync(1);
    }

    private async Task PrevPage()
    {
        if (CurrentPage > 1)
            await CurrentPageChanged.InvokeAsync(CurrentPage - 1);
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
            await CurrentPageChanged.InvokeAsync(CurrentPage + 1);
    }

    private async Task LastPage()
    {
        if (CurrentPage < TotalPages)
            await CurrentPageChanged.InvokeAsync(TotalPages);
    }

    private async Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var size))
        {
            await PageSizeChanged.InvokeAsync(size);
            await CurrentPageChanged.InvokeAsync(1);
        }
    }
}

