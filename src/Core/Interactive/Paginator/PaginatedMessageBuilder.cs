using System.Collections.Generic;
using System.Linq;
using Discord;
using Gommon;
using Volte.Commands;
using Volte.Core;

namespace Volte.Interactive
{
    public class PaginatedMessageBuilder
    {
        public IEnumerable<object> Pages { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public IGuildUser Author { get; private set; }
        public Color Color { get; private set; } = new Color(Config.SuccessColor);
        public string Title { get; private set; }
        public string AlternateDescription { get; private set; } = string.Empty;
        public PaginatedAppearanceOptions Options { get; private set; } = PaginatedAppearanceOptions.Default;
        
        public static PaginatedMessageBuilder New => new PaginatedMessageBuilder();

        public PaginatedMessageBuilder WithPages(IEnumerable<object> pages)
        {
            Pages = pages;
            return this;
        }

        public PaginatedMessageBuilder WithContent(string text)
        {
            Content = text;
            return this;
        }
        
        public PaginatedMessageBuilder WithAuthor(IGuildUser user)
        {
            Author = user;
            return this;
        }
        
        public PaginatedMessageBuilder WithColor(Color color)
        {
            Color = color;
            return this;
        }
        
        public PaginatedMessageBuilder WithTitle(string text)
        {
            Title = text;
            return this;
        }
        
        public PaginatedMessageBuilder WithAlternateDescription(string text)
        {
            AlternateDescription = text;
            return this;
        }
        
        public PaginatedMessageBuilder WithOptions(PaginatedAppearanceOptions options)
        {
            Options = options;
            return this;
        }

        public PaginatedMessageBuilder WithDefaults(VolteContext ctx)
        {
            Author = ctx.User;
            Color = ctx.User.GetHighestRoleWithColor()?.Color ?? new Color(Config.SuccessColor);
            return this;
        }

        public PaginatedMessageBuilder SplitPages(int perPage)
        {
            var temp = Pages.ToList();
            var newList = new List<object>();

            do
            {
                newList.Add(temp.Take(perPage).Select(x => x.ToString()).Join("\n"));
                temp.RemoveRange(0, temp.Count < perPage ? temp.Count : perPage);
            } while (!temp.IsEmpty());

            Pages = newList;
            return this;
        }

        public PaginatedMessage Build()
        {
            return new PaginatedMessage
            {
                Pages = Pages,
                Content = Content,
                Author = Author,
                Color = Color,
                Title = Title,
                AlternateDescription = AlternateDescription,
                Options = Options
            };
        }
        
    }
}