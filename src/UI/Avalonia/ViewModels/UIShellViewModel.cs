using CommunityToolkit.Mvvm.ComponentModel;
using Volte.UI.Avalonia.Views;

namespace Volte.UI.Avalonia.ViewModels;

// ReSharper disable once InconsistentNaming
public partial class UIShellViewModel : BaseModel
{
    public required UIShellView? View { get; init; } 

    [ObservableProperty]
    private string _connection = "Disconnected";

    public UIShellViewModel()
    {
        if (VolteBot.Client is null) return;
        
        VolteBot.Client.Connected += ChangeConnectionState;
        VolteBot.Client.Disconnected += Disconnected;
    }

    ~UIShellViewModel()
    {
        if (VolteBot.Client is null) return;
        
        VolteBot.Client.Connected -= ChangeConnectionState;
        VolteBot.Client.Disconnected -= Disconnected;
    }

    private Task ChangeConnectionState()
    {
        Connection = VolteManager.GetConnectionState();
        return Task.CompletedTask;
    }
    
    private Task Disconnected(Exception e)
    {
        VolteApp.NotifyError(e);
        return ChangeConnectionState();
    }
}