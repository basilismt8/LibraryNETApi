using LibraryBlazor.Features.Books.Models;
using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Components;

public partial class HistoryModal
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public string? Error { get; set; }
    [Parameter] public Dictionary<Guid, List<LoanHistoryEventDto>>? Events { get; set; }
    [Parameter] public bool SingleCopyMode { get; set; }
    [Parameter] public string? CopyCode { get; set; }
    [Parameter] public string? BookTitle { get; set; }

    private Guid? _selectedCopyId;

    protected override void OnParametersSet()
    {
        if (SingleCopyMode && Events != null && Events.Count == 1)
            _selectedCopyId = Events.Keys.First();
        else if (!SingleCopyMode)
            _selectedCopyId = null;
    }

    private void ShowCopyHistory(Guid copyId) => _selectedCopyId = copyId;
    private void BackToCopyList() => _selectedCopyId = null;

    private Task Close()
    {
        _selectedCopyId = null;
        return IsOpenChanged.InvokeAsync(false);
    }
}
