// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace DarlingWeb.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using DarlingWeb;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\_Imports.razor"
using DarlingWeb.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\Pages\Guild.razor"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\Pages\Guild.razor"
           [Authorize]

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Guilds/{Id}")]
    public partial class Guild : LayoutComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 191 "C:\Users\XXXPOSOSI\source\repos\DarlingNet\DarlingWeb\Pages\Guild.razor"
       
    [Parameter]
    public string Id { get; set; }
    public DarlingWeb.Data.Models.Guild DscGuild { get; set; }
    public List<DarlingWeb.Data.Models.Channel> DscChannel { get; set; }
    public State state = State.Loading;
    public bool PrivateChannelSelected { get; set; }
    public string PrivateChannelId { get; set; }
    public enum State
    {
        Loading,
        Unavailable,
        Loaded
    }
    DarlingDb.Models.Guilds StableGuild = new DarlingDb.Models.Guilds();
    DarlingDb.Models.Guilds ThisGuild = new DarlingDb.Models.Guilds();
    DarlingDb.Models.Channel VoiceChannel = new DarlingDb.Models.Channel();



    async Task HandleSubmit()
    {
        ulong Id = Convert.ToUInt64(PrivateChannelId);
        if(ThisGuild.Prefix != StableGuild.Prefix || ThisGuild.PrivateId != StableGuild.PrivateId)
            await GuildService.UpdateGuild(ThisGuild, Convert.ToUInt64(Id));

    }

    //Use OnParametersSet since the guild id can be changed without the page re-rendering the new content otherwise
    protected override async Task OnParametersSetAsync()
    {
        DscGuild = null;
        StableGuild = null;
        DscChannel = null;
        PrivateChannelSelected = false;
        state = State.Loading;

        if (Id == null || Id == "0")
        {
            //TODO: show all available guilds?
            //Probably not needed since guilds are now populated on the sidebar
            state = State.Unavailable;
        }
        else    
        {
            DscGuild = await GetGuild(Id);
            if (DscGuild == null)
                state = State.Unavailable;
            else
            {
                ulong Ids = Convert.ToUInt64(Id);
                    ThisGuild = await GuildService.GetGuild(Ids);
                    DscChannel = await usrSvc.GetChannelsGuildsAsync(httpContextAccessor.HttpContext, Ids);
                    StableGuild = ThisGuild;
                    state = State.Loaded;
                
                
            }
        }
        
    }

    private async Task<Data.Models.Guild> GetGuild(string id)
    {
        var guilds = await usrSvc.GetBotGuildsAsync(httpContextAccessor.HttpContext);
        var match = guilds.FirstOrDefault(x => x.Id == id);
        return match;
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private Data.IGuildService GuildService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DarlingWeb.Data.UserService usrSvc { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IHttpContextAccessor httpContextAccessor { get; set; }
    }
}
#pragma warning restore 1591
