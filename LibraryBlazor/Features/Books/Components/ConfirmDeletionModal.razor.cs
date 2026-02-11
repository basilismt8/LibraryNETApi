using Microsoft.AspNetCore.Components;

namespace LibraryBlazor.Features.Books.Components
{
    public partial class ConfirmDeletionModal
    {
        [Parameter] public bool IsOpen { get; set; }
        [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter] public int? numberOfSelectedBooks { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }
        [Parameter] public EventCallback OnYes { get; set; }

        protected string Message => numberOfSelectedBooks > 1 ? "Are you sure that you want to delete this books?" : "Are you sure that you want to delete this book?";
        protected async Task HandleCancelAsync()
        {
            await OnCancel.InvokeAsync();
        }
    }
}
