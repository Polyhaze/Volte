using FluentAvalonia.UI.Controls;
using Volte.UI.Avalonia.Controls;
using Volte.UI.Avalonia.ViewModels;
using Volte.UI.Helpers;

namespace Volte.UI.Avalonia.Views;

[UiPage(PageType.Guilds, "Manage Guilds", Symbol.Admin)]
public partial class GuildsView : VolteControl<GuildsViewModel>
{
    public GuildsView()
    {
        InitializeComponent();
        ViewModel = new GuildsViewModel();
        
        LeaveSelectedGuildButton.Command = Commands.Create(ViewModel.LeaveSelectedGuildAsync);
        DeleteSelectedGuildDataButton.Command = Commands.Create(ViewModel.ClearSelectedGuildDataAsync);
        SendMessageToGuildOwnerButton.Command = Commands.Create(ViewModel.MessageGuildOwnerAsync);
    }
}