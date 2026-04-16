namespace LibraryBlazor.Components;

public partial class GuidDisplay
{
    [Microsoft.AspNetCore.Components.Parameter, Microsoft.AspNetCore.Components.EditorRequired]
    public Guid Value { get; set; }

    private string ShortLabel =>
        Value.ToString("N")[..4] + "…" + Value.ToString("N")[^4..];

    private bool _visible;

    private void ShowTooltip() => _visible = true;
    private void HideTooltip() => _visible = false;
}
