using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Components;

public partial class ConfirmationModal
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter] public string Title { get; set; } = "Confirmation";
    [Parameter] public string Message { get; set; } = string.Empty;
    [Parameter] public bool IsWarning { get; set; } = false;

    [Parameter] public string CancelText { get; set; } = "Cancel";
    [Parameter] public string ConfirmText { get; set; } = "Yes";

    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnConfirm { get; set; }

    protected string MessageCssClass => IsWarning ? "confirmation-message warning" : "confirmation-message";

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            Message = string.Empty;
        }
    }

    protected async Task HandleCancelAsync()
    {
        await IsOpenChanged.InvokeAsync(false);
        await OnCancel.InvokeAsync();
    }

    protected async Task HandleConfirmAsync()
    {
        await IsOpenChanged.InvokeAsync(false);
        await OnConfirm.InvokeAsync();
    }
}
