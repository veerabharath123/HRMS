using Aspose.Words;
using Aspose.Words.Replacing;
using HRMS.Domain.Common;
using System.Drawing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    public class ReplaceWithTextHandler : IReplacingCallback
    {
        private readonly DocTemplateTextField _field;
        public ReplaceWithTextHandler(DocTemplateTextField field) => _field = field;

        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
        {
            Document document = (Document)args.MatchNode.Document;
            var (firstRun, runs, index) = ReplaceWithHandler.CollectAndRemoveMatch(document.Range, args);

            if (NeedsFallback(firstRun, index))
            {
                InsertFallbackText(document, args.MatchNode, _field);
                return ReplaceAction.Skip;
            }

            InsertStyledRun(document, firstRun!, index, _field);
            CleanupEmptyRuns(runs);
            return ReplaceAction.Skip;
        }

        private static bool NeedsFallback(Run? firstRun, int index) =>
            firstRun is null || index < 0;

        private static void InsertFallbackText(Document doc, Node matchNode, DocTemplateTextField field)
        {
            var builder = new DocumentBuilder(doc);
            builder.MoveTo(matchNode);
            ApplyFontStyle(builder.Font, field.Style, (matchNode as Run)?.Font);
            matchNode.Remove();
            builder.Write(field.Value ?? string.Empty);
        }

        private static void InsertStyledRun(Document doc, Run firstRun, int index, DocTemplateTextField field)
        {
            var newRun = new Run(doc, field.Value ?? string.Empty);
            ApplyFontStyle(newRun.Font, field.Style, firstRun.Font);

            if (index > 0)
                firstRun.ParentNode.InsertAfter(newRun, firstRun);
            else
                firstRun.ParentNode.InsertBefore(newRun, firstRun);
        }

        private static void CleanupEmptyRuns(IEnumerable<Run> runs)
        {
            foreach (var run in runs.Where(r => string.IsNullOrEmpty(r.Text)).ToList())
                run.Remove();
        }

        private static void ApplyFontStyle(Font target, DocTextStyle? style, Font? placeholder)
        {
            CopyPlaceholderStyle(target, placeholder);
            ApplyOverrides(target, style);
        }

        private static void CopyPlaceholderStyle(Font target, Font? placeholder)
        {
            if (placeholder == null) return;
            target.Name = placeholder.Name;
            target.Size = placeholder.Size;
            target.Bold = placeholder.Bold;
            target.Italic = placeholder.Italic;
            target.Color = placeholder.Color;
        }

        private static void ApplyOverrides(Font target, DocTextStyle? style)
        {
            if (style is null) return;
            target.Name = string.IsNullOrWhiteSpace(style.FontName) ? target.Name : style.FontName;
            target.Size = style.FontSize ?? target.Size;
            target.Bold = style.Bold ?? target.Bold;
            target.Italic = style.Italic ?? target.Italic;
            target.Color = string.IsNullOrWhiteSpace(style.ColorHex)
                ? target.Color
                : ColorTranslator.FromHtml(style.ColorHex);
        }
    }

}
