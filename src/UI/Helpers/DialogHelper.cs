using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;

namespace Volte.UI.Helpers;

public static class DialogHelper
{
    internal static async Task<bool> CreateConfirmationDialog(
        string primaryText,
        string secondaryText,
        string acceptButtonText,
        string cancelButtonText,
        string? title = null,
        UserResult primaryButtonResult = UserResult.Yes)
        => await ShowTextDialog(
            string.IsNullOrWhiteSpace(title) ? "Volte - Confirm" : title,
            primaryText,
            secondaryText,
            acceptButtonText,
            string.Empty,
            cancelButtonText,
            (int)Symbol.Help,
            primaryButtonResult) == primaryButtonResult;
    
    
    public static ContentDialog ApplyStyles(
        this ContentDialog contentDialog,
        double closeButtonWidth = 80,
        HorizontalAlignment buttonSpaceAlignment = HorizontalAlignment.Right)
    {
        Style closeButton = new(x => x.Name("CloseButton"));
        closeButton.Setters.Add(new Setter(Layoutable.WidthProperty, closeButtonWidth));

        Style closeButtonParent = new(x => x.Name("CommandSpace"));
        closeButtonParent.Setters.Add(new Setter(Layoutable.HorizontalAlignmentProperty, buttonSpaceAlignment));

        contentDialog.Styles.Add(closeButton);
        contentDialog.Styles.Add(closeButtonParent);

        return contentDialog;
    }

    private static async Task<UserResult> ShowContentDialog(
        string title,
        object content,
        string primaryButton,
        string secondaryButton,
        string closeButton,
        UserResult primaryButtonResult = UserResult.Ok,
        ManualResetEvent? deferResetEvent = null,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? deferCloseAction = null)
    {
        UserResult result = UserResult.None;

        ContentDialog contentDialog = new()
        {
            Title = title,
            PrimaryButtonText = primaryButton,
            SecondaryButtonText = secondaryButton,
            CloseButtonText = closeButton,
            Content = content,
            PrimaryButtonCommand = Commands.Create(() => { result = primaryButtonResult; })
        };

        contentDialog.SecondaryButtonCommand = Commands.Create(() =>
        {
            result = UserResult.No;
            contentDialog.PrimaryButtonClick -= deferCloseAction;
        });

        contentDialog.CloseButtonCommand = Commands.Create(() =>
        {
            result = UserResult.Cancel;
            contentDialog.PrimaryButtonClick -= deferCloseAction;
        });

        if (deferResetEvent != null)
        {
            contentDialog.PrimaryButtonClick += deferCloseAction;
        }

        await contentDialog.ShowAsync();

        return result;
    }

    public static async Task<(UserResult Result, string? Input)> ShowTextInputDialog(
        string title,
        string primaryText,
        string secondaryText,
        string primaryButton,
        string secondaryButton,
        string closeButton,
        string textBoxWatermark,
        UserResult primaryButtonResult = UserResult.Ok,
        ManualResetEvent? deferResetEvent = null,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? deferCloseAction = null)
    {
        var (content, getText) = CreateTextInputDialogContent(textBoxWatermark, primaryText, secondaryText);

        var result = await ShowContentDialog(title, content, primaryButton, secondaryButton, closeButton, primaryButtonResult,
            deferResetEvent, deferCloseAction);

        return (result, getText());
    }
    
    public static async Task<UserResult> ShowTextDialog(
        string title,
        string primaryText,
        string secondaryText,
        string primaryButton,
        string secondaryButton,
        string closeButton,
        int iconSymbol,
        UserResult primaryButtonResult = UserResult.Ok,
        ManualResetEvent? deferResetEvent = null,
        TypedEventHandler<ContentDialog, ContentDialogButtonClickEventArgs>? deferCloseAction = null)
    {
        Grid content = CreateTextDialogContent(primaryText, secondaryText, iconSymbol);

        return await ShowContentDialog(title, content, primaryButton, secondaryButton, closeButton, primaryButtonResult,
            deferResetEvent, deferCloseAction);
    }

    public static async Task<UserResult> ShowDeferredContentDialog(
        Window window,
        string title,
        string primaryText,
        string secondaryText,
        string primaryButton,
        string secondaryButton,
        string closeButton,
        int iconSymbol,
        ManualResetEvent deferResetEvent,
        Func<Window, Task>? doWhileDeferred = null)
    {
        bool startedDeferring = false;

        return await ShowTextDialog(
            title,
            primaryText,
            secondaryText,
            primaryButton,
            secondaryButton,
            closeButton,
            iconSymbol,
            primaryButton == "Yes" ? UserResult.Yes : UserResult.Ok,
            deferResetEvent,
            DeferClose);

        async void DeferClose(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (startedDeferring)
            {
                return;
            }

            sender.PrimaryButtonClick -= DeferClose;

            startedDeferring = true;

            Deferral deferral = args.GetDeferral();

            sender.PrimaryButtonClick -= DeferClose;

            _ = Task.Run(() =>
            {
                deferResetEvent.WaitOne();

                Dispatcher.UIThread.Post(() => { deferral.Complete(); });
            });

            if (doWhileDeferred != null)
            {
                await doWhileDeferred(window);

                deferResetEvent.Set();
            }
        }
    }

    private static (Grid Content, Func<string?> GetInputText) CreateTextInputDialogContent(string textBoxWatermark, string? primaryText = null, string? secondaryText = null)
    {
        Grid content = new()
        {
            RowDefinitions = [new(), new(), new()],
            ColumnDefinitions = [new()],

            MinHeight = 80,
        };

        if (!string.IsNullOrEmpty(primaryText))
        {
            TextBlock primaryLabel = new()
            {
                Text = primaryText,
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450,
            };
            
            Grid.SetRow(primaryLabel, 0);
            content.Children.Add(primaryLabel);
        }

        if (!string.IsNullOrEmpty(secondaryText))
        {
            TextBlock secondaryLabel = new()
            {
                Text = secondaryText,
                Margin = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 450,
            };

            Grid.SetRow(secondaryLabel, 1);
            content.Children.Add(secondaryLabel);
        }

        TextBox inputBox = new()
        {
            Margin = new Thickness(5),
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 450,
            Watermark = textBoxWatermark
        };

        Grid.SetRow(inputBox, 2);

        content.Children.Add(inputBox);

        return (content, () => inputBox.Text);
    }
    
    private static Grid CreateTextDialogContent(string primaryText, string secondaryText, int symbol)
    {
        Grid content = new()
        {
            RowDefinitions = [new(), new()],
            ColumnDefinitions = [new(GridLength.Auto), new()],

            MinHeight = 80,
        };

        SymbolIcon icon = new()
        {
            Symbol = (Symbol)symbol,
            Margin = new Thickness(10),
            FontSize = 40,
            FlowDirection = FlowDirection.LeftToRight,
            VerticalAlignment = VerticalAlignment.Center,
        };

        Grid.SetColumn(icon, 0);
        Grid.SetRowSpan(icon, 2);
        Grid.SetRow(icon, 0);

        TextBlock primaryLabel = new()
        {
            Text = primaryText,
            Margin = new Thickness(5),
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 450,
        };

        TextBlock secondaryLabel = new()
        {
            Text = secondaryText,
            Margin = new Thickness(5),
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 450,
        };

        Grid.SetColumn(primaryLabel, 1);
        Grid.SetColumn(secondaryLabel, 1);
        Grid.SetRow(primaryLabel, 0);
        Grid.SetRow(secondaryLabel, 1);

        content.Children.Add(icon);
        content.Children.Add(primaryLabel);
        content.Children.Add(secondaryLabel);

        return content;
    }
}

public enum UserResult
{
    Ok,
    Yes,
    No,
    Abort,
    Cancel,
    None,
}