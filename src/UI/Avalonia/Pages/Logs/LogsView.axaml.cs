using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Volte.Helpers;
using Volte.UI.Helpers;

namespace Volte.UI.Avalonia.Pages;

[UiPage(PageType.Logs, "Bot Logs", Symbol.AllApps, isDefault: true, isFooter: true)]
public partial class LogsView : UserControl
{
    public LogsView()
    {
        InitializeComponent();
        DataContext = new LogsViewModel { View = this, LogsClearAmount = 10 };
        
        CopySimpleIcon.Value = FontAwesome.Copy;
        CopySimple.Command = new AsyncRelayCommand(async () =>
        {
            if (this.Context<LogsViewModel>().Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.FormattedString);
        });

        CopyMarkdownIcon.Value = FontAwesome.Brush;
        CopyMarkdown.Command = new AsyncRelayCommand(async () =>
        {
            if (this.Context<LogsViewModel>().Selected is { } selected)
                await OS.CopyToClipboardAsync(selected.Markdown);
        });
    }
}