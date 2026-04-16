using Microsoft.AspNetCore.Components;
using LibraryBlazor.Features.Books.Models;

namespace LibraryBlazor.Features.Books.Components;

public partial class BooksTable
{
    [Parameter, EditorRequired] public List<BookRowVm> Items { get; set; } = new();

    [Parameter] public string FilterId { get; set; } = "";
    [Parameter] public EventCallback<string> FilterIdChanged { get; set; }

    [Parameter] public string FilterTitle { get; set; } = "";
    [Parameter] public EventCallback<string> FilterTitleChanged { get; set; }

    [Parameter] public int CurrentPage { get; set; } = 1;
    [Parameter] public EventCallback<int> CurrentPageChanged { get; set; }

    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

    [Parameter] public EventCallback OnNew { get; set; }
    [Parameter] public EventCallback OnEdit { get; set; }
    [Parameter] public EventCallback OnDelete { get; set; }
    [Parameter] public EventCallback OnLoan { get; set; }


    private IEnumerable<BookRowVm> FilteredItems =>
        Items.Where(b =>
            (string.IsNullOrWhiteSpace(FilterId) || b.Id.ToString().Contains(FilterId, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrWhiteSpace(FilterTitle) || b.Title.Contains(FilterTitle, StringComparison.OrdinalIgnoreCase)));

    private IEnumerable<BookRowVm> PagedItems =>
        FilteredItems.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredItems.Count() / (double)PageSize));

    private bool CanEdit => Items.Count(b => b.IsSelected) == 1;
    private bool CanDelete => Items.Any(b => b.IsSelected);


    private bool SelectAllRows
    {
        get => FilteredItems.Any() && FilteredItems.All(b => b.IsSelected);
        set
        {
            foreach (var b in FilteredItems)
                b.IsSelected = value;
        }
    }

    private string GetRowClass(BookRowVm book) => book.IsSelected ? "row-selected" : "";

    private void ToggleSelection(BookRowVm book) => book.IsSelected = !book.IsSelected;

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