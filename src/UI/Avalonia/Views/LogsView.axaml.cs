using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Volte.Helpers;
using Volte.UI.Avalonia.Controls;
using Volte.UI.Avalonia.ViewModels;
using Volte.UI.Helpers;

namespace Volte.UI.Avalonia.Views;

[UiPage(PageType.Logs, "Bot Logs", Symbol.AllApps, isDefault: true, isFooter: true)]
public partial class LogsView : VolteControl<LogsViewModel>
{
    public LogsView()
    {
        InitializeComponent();
        ViewModel = new LogsViewModel { View = this, LogsClearAmount = 10 };
        
        CopySimpleIcon.Value = FontAwesome.Copy;
        CopySimple.Command = new AsyncRelayCommand(async () =>
        {
            if (ViewModel.Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.FormattedString);
        });

        CopyMarkdownIcon.Value = FontAwesome.Brush;
        CopyMarkdown.Command = new AsyncRelayCommand(async () =>
        {
            if (ViewModel.Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.Markdown);
        });
    }
}