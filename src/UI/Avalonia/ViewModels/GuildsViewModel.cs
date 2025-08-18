using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Discord.WebSocket;
using Gommon;
using Volte.Helpers;
using Volte.Systems.Database;
using Volte.Systems.Database.EntitiesV2;
using Volte.UI.Avalonia.Models;
using Volte.UI.Helpers;

namespace Volte.UI.Avalonia.ViewModels;

public partial class GuildsViewModel : BaseModel
{
    [ObservableProperty]
    private IEnumerable<GuildModel> _guilds = null!;

    [ObservableProperty]
    private bool _selected;

    private GuildModel? _selectedGuild;
    
    public GuildModel? SelectedGuild
    {
        get => _selectedGuild;
        set
        {
            OnPropertyChanging();

            _selectedGuild = value;

            Selected = _selectedGuild != null;

            OnPropertyChanged();
        }
    }

    public GuildsViewModel()
    {
        VolteBot.Client.JoinedGuild += HandleGuildUpdate;
        VolteBot.Client.LeftGuild += HandleGuildUpdate;
        VolteBot.Client.GuildAvailable += HandleGuildUpdate;
        VolteBot.Client.GuildUnavailable += HandleGuildUpdate;
    }
    
    ~GuildsViewModel()
    {
        VolteBot.Client.JoinedGuild -= HandleGuildUpdate;
        VolteBot.Client.LeftGuild -= HandleGuildUpdate;
        VolteBot.Client.GuildAvailable -= HandleGuildUpdate;
        VolteBot.Client.GuildUnavailable -= HandleGuildUpdate;
    }

    public async Task LeaveSelectedGuildAsync()
    {
        if (SelectedGuild is null) return;
        
        var guildName = SelectedGuild.Entity.Name;
        var guildId = SelectedGuild.Entity.Id;

        if (await ConfirmAsync($"About to leave the guild '{guildName}'"))
        {
            await SelectedGuild.Entity.LeaveAsync();
            VolteApp.Notify($"Left guild '{guildName}' ({guildId})");
        }
    }
    
    public async Task ClearSelectedGuildDataAsync()
    {
        if (SelectedGuild is null) return;
        
        if (await ConfirmAsync($"About to reset the configuration & data for guild '{SelectedGuild.Entity.Name}'"))
        {
            var db = VolteBot.Services.Get<DatabaseService>();
            db.Save(GuildDataV2.CreateFrom(SelectedGuild.Entity));
            VolteApp.Notify($"Reset guild data for '{SelectedGuild.Entity.Name}'");
        }
    }
    
    public async Task MessageGuildOwnerAsync()
    {
        if (SelectedGuild is null) return;
        
        var (result, message) = await DialogHelper.ShowTextInputDialog(
            title: $"Send a message to '{SelectedGuild.Entity.Owner.Username}'",
            primaryText: "What do you want to send?",
            secondaryText: string.Empty,
            primaryButton: "Ok",
            secondaryButton: string.Empty,
            closeButton: "Cancel",
            textBoxWatermark: "Hello from Volte UI!");
        
        if (result is UserResult.Ok)
        {
            if (message.IsNullOrEmpty())
                VolteApp.Notify("Can't send an empty message!", $"Could not message '{SelectedGuild.Entity.Owner.Username}'", type: NotificationType.Warning);
            else if (await SelectedGuild.Entity.Owner.TrySendMessageAsync(message))
                VolteApp.Notify($"Recipient: {SelectedGuild.Entity.Owner.Username}", "Message Sent");
            else
                VolteApp.Notify(
                    "Either they have the bot blocked, or have DMs disabled.", 
                    $"Could not message '{SelectedGuild.Entity.Owner.Username}'", 
                    type: NotificationType.Error);
        }
    }

    private static Task<bool> ConfirmAsync(string action) =>
        DialogHelper.CreateConfirmationDialog(
            primaryText: action,
            secondaryText: "Do you wish to continue? Click 'No' to cancel.",
            acceptButtonText: "Yes",
            cancelButtonText: "No");

    private Task HandleGuildUpdate(SocketGuild guild) => Dispatcher.UIThread.InvokeAsync(() =>
    {
        Guilds = GuildModel.MapDataToGuilds(
            VolteBot.Services.Get<DatabaseService>().GetAllData(),
            VolteBot.Client.Guilds
        );
        
        return Task.CompletedTask;
    });
}