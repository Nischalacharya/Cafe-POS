using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Cafe.POS.Services;

namespace Cafe.POS.Components.Layout;

public partial class MainLayout
{
    private readonly GlobalState _globalState = new();

    protected override void OnInitialized()
    {
        UserService.SeedUser();
    }

    private void LogoutHandler()
    {
        _globalState.User = null;

        NavManager.NavigateTo("/login");
    }
}