using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Components;

public partial class DueDateModal
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter] public bool Saving { get; set; }
    [Parameter] public string? Error { get; set; }

    [Parameter] public string Title { get; set; } = "Select Due Date";
    [Parameter] public string? SubjectLabel { get; set; }
    [Parameter] public string? SubjectValue { get; set; }
    [Parameter] public string DueDateLabel { get; set; } = "Due Date";
    [Parameter] public DateOnly? CurrentDueDate { get; set; }

    [Parameter] public DateOnly NewDueDate { get; set; }
    [Parameter] public EventCallback<DateOnly> NewDueDateChanged { get; set; }

    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnConfirm { get; set; }

    protected async Task HandleCancelAsync()
    {
        await OnCancel.InvokeAsync();
    }

    protected async Task HandleConfirmAsync()
    {
        await OnConfirm.InvokeAsync();
    }

    protected async Task HandleNewDueDateChangedAsync(ChangeEventArgs e)
    {
        if (DateOnly.TryParse(e.Value?.ToString(), out var date))
        {
            NewDueDate = date;
            await NewDueDateChanged.InvokeAsync(NewDueDate);
        }
    }
}
