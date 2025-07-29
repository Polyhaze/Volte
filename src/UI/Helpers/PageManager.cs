using System.Collections.ObjectModel;
using System.Reflection;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Discord.Interactions;
using FluentAvalonia.UI.Controls;
using Gommon;

namespace Volte.UI.Helpers;

public partial class PageManager : ObservableObject
{
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<PageManager> _shared = new(() => new PageManager());
    public static PageManager Shared => _shared.Value;

    [ObservableProperty] private Page? _current;

    private readonly Dictionary<PageType, (int Index, bool IsFooter)> _lookup = [];
    public ObservableCollection<Page> Pages { get; } = [];
    public ObservableCollection<Page> FooterPages { get; } = [];

    public Page this[PageType pageType]
    {
        get
        {
            var (index, isFooter) = _lookup[pageType];
            return (isFooter ? FooterPages : Pages)[index];
        }
    }

    public void Register(PageType pageType, string title, object? content, Symbol icon, string? description = null,
        bool isDefault = false, bool isFooter = false)
    {
        var source = isFooter ? FooterPages : Pages;
        _lookup[pageType] = (source.Count, isFooter);

        source.Add(new Page
        {
            Title = title,
            Content = content,
            Description = description,
            Icon = icon
        });

        if (isDefault)
            Focus(pageType);
    }

    public void Focus(PageType pageType) => Current = this[pageType];


    public bool TryGetViewModel<T>(out T viewModel) where T : ObservableObject
    {
        foreach (var page in Pages.Concat(FooterPages))
            if (page.Content is UserControl { DataContext: T pageViewModel })
            {
                viewModel = pageViewModel;
                return true;
            }

        viewModel = null!;
        return false;
    }

    public T GetViewModel<T>() where T : ObservableObject
        => TryGetViewModel<T>(out var result)
            ? result
            : throw new InvalidOperationException(
                $"No registered page has the DataContext type of '{typeof(T).AsFullNamePrettyString()}'");


    #region Attributes

    public static void Init(Optional<Assembly> assembly = default)
    {
        assembly.OrElse(Assembly.GetExecutingAssembly()).GetTypes()
            .Where(static x => x.HasAttribute<UiPageAttribute>() && !x.HasAttribute<DontAutoRegisterAttribute>())
            .Select(static x => (Type: x, Meta: x.GetCustomAttribute<UiPageAttribute>()!))
            .OrderBy(static x => x.Meta.PageType)
            .ForEach(static page => Shared.Register(
                pageType: page.Meta.PageType,
                title: page.Meta.Title,
                content: page.Type.GetConstructor(Type.EmptyTypes)?.Invoke([])
                         ?? throw new TypeInitializationException(
                             page.Type.AsFullNamePrettyString(),
                             new Exception($"""
                                            The specified type does not have a parameterless constructor.
                                            Create one or use {nameof(DontAutoRegisterAttribute)}
                                            on the page type to prevent auto-registration.
                                            """.ReplaceLineEndings())
                         ),
                icon: page.Meta.Icon,
                description: page.Meta.Description,
                isDefault: page.Meta.IsDefault,
                isFooter: page.Meta.IsFooter
            ));
    }

    #endregion
}

public enum PageType : byte
{
    Logs = 0
}

public class Page
{
    public required string Title { get; set; }
    public required object? Content { get; set; }
    public required Symbol Icon { get; set; }
    public required string? Description { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class UiPageAttribute : Attribute
{
    public UiPageAttribute(PageType pageType, string description, Symbol icon, string? title = null,
        bool isDefault = false, bool isFooter = false)
    {
        PageType = pageType;
        Title = title ?? Enum.GetName(pageType)!;
        Description = description;
        Icon = icon;
        IsDefault = isDefault;
        IsFooter = isFooter;
    }

    public string Title { get; }
    public string Description { get; }
    public PageType PageType { get; }
    public Symbol Icon { get; }
    public bool IsDefault { get; }
    public bool IsFooter { get; }
}