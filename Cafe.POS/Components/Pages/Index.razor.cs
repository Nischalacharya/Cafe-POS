using Cafe.POS.Models;
using Cafe.POS.Models.Base;
using Microsoft.AspNetCore.Components;

namespace Cafe.POS.Components.Pages;

public partial class Index
{
    [CascadingParameter] private GlobalState _globalState { get; set; }

    protected override void OnInitialized()
    {
        NavManager.NavigateTo(_globalState.User == null ? "/login" : "/home");
    }
}