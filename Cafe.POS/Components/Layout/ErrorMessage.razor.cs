using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Layout;

public partial class ErrorMessage
{
    [Parameter] public string Type { get; set; }

    [Parameter] public string Message { get; set; }
}