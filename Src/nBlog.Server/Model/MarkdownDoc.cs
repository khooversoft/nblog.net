using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace NBlog.Server.Model
{
    public record MarkdownDoc
    {
        private string? _html;

        public MarkdownDoc(byte[] data)
        {
            data.VerifyNotNull(nameof(data));

            MdSource = Encoding.UTF8.GetString(data.RemoveBOM());
        }

        public string MdSource { get; init; }

        public string ToHtml()
        {
            return _html ??= build();

            string build()
            {
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Build();

                return Markdown.ToHtml(MdSource, pipeline);
            }
        }
    }
}
