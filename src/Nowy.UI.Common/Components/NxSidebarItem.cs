using Havit.Blazor.Components.Web.Bootstrap;

namespace Nowy.UI.Common.Components;

public class NxSidebarItem : HxSidebarItem
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && this.collapseComponent is { })
        {
            await this.collapseComponent.ShowAsync();
        }
    }
}
