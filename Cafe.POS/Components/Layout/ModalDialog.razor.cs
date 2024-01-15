using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Layout;

public partial class ModalDialog
{
    [Parameter] 
    public string Title { get; set; }

    [Parameter] 
    public RenderFragment ChildContent { get; set; }

    [Parameter] 
    public string OkLabel { get; set; }

    [Parameter] 
    public string Size { get; set; } = "";

    [Parameter] 
    public EventCallback<bool> OnClose { get; set; }

    private Task ModalCancel()
    {
        return OnClose.InvokeAsync(true);
    }

    private Task ModalOk()
    {
        return OnClose.InvokeAsync(false);
    }
}