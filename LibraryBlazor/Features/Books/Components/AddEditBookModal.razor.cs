using LibraryBlazor.Features.Books.Models;
using Microsoft.AspNetCore.Components;
using System;

namespace LibraryBlazor.Features.Books.Components;

public partial class AddEditBookModal
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter] public bool Saving { get; set; }
    [Parameter] public string? Error { get; set; }

    [Parameter] public Guid? EditingId { get; set; }

    [Parameter, EditorRequired] public BookEditModel Model { get; set; } = new();

    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }

    protected string Title => EditingId is null ? "Add New Book" : "Edit Book";

    protected async Task HandleCancelAsync()
    {
        await OnCancel.InvokeAsync();
    }
}