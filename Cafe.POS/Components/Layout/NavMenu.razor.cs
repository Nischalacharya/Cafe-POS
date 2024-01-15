using Cafe.POS.Models;
using Cafe.POS.Models.Enums;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Layout;

public partial class NavMenu
{
    [Parameter]
    public string Username { get; set; } = null!;

    [Parameter] 
    public Role Role { get; set; }

    [Parameter] 
    public EventCallback LogoutHandler { get; set; }

    private bool CollapseNavMenu { get; set; } = true;
    
    private string? NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

    private void ToggleNavMenu()
    {
        CollapseNavMenu = !CollapseNavMenu;
    }
}