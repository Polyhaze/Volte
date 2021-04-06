using System.Collections.Generic;
using System.Linq;
using Discord;
using Gommon;

namespace Volte.Interactive
{
    public class PaginatedMessage
    {
        /// <summary>
        /// Pages contains a collection of elements to page over in the embed. It is expected
        /// that a string-like object is used in this collection, as objects will be converted
        /// to a displayable string only through their generic ToString method, with the
        /// exception of EmbedFieldBuilders.
        /// 
        /// If this collection is of <see cref="EmbedFieldBuilder"/>, then the pages will be displayed in
        /// batches of <see cref="PaginatedAppearanceOptions.FieldsPerPage"/>, and the
        /// embed's description will be populated with the <see cref="AlternateDescription"/> field.
        ///
        /// If this collection is of <see cref="EmbedBuilder"/>, every setting in <see cref="PaginatedMessage"/> will be ignored as
        /// the contents of the EmbedBuilders are used instead.
        /// </summary>
        public IEnumerable<object> Pages { get; set; }

        /// <summary>
        /// Content sets the content of the message, displayed above the embed. This may remain empty.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Author sets the <see cref="EmbedBuilder.Author"/> property directly.
        /// </summary>
        public IGuildUser Author { get; set; } = null;
        public Color Color { get; set; } = Color.Default;
        public string Title { get; set; }
        /// <summary>
        /// AlternateDescription will be used as the description of the pager only when
        /// <see cref="Pages"/> is a collection of <see cref="EmbedFieldBuilder"/>.
        /// </summary>
        public string AlternateDescription { get; set; } = string.Empty;

        public PaginatedAppearanceOptions Options { get; set; } = PaginatedAppearanceOptions.Default;

        public PaginatedMessage SplitPagesBy(int entriesPerPage)
        {
            var temp = Pages.ToList();
            var newList = new List<object>();

            do
            {
                newList.Add(temp.Take(entriesPerPage).Select(x => x.ToString()).Join("\n"));
                temp.RemoveRange(0, temp.Count < entriesPerPage ? temp.Count : entriesPerPage);
            } while (!temp.IsEmpty());

            Pages = newList;

            return this;
        }
    }
}
