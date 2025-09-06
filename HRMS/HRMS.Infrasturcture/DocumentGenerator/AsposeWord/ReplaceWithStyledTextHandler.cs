using Aspose.Words;
using Aspose.Words.Replacing;
using HRMS.Domain.Common;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HRMS.Infrasturcture.DocumentGenerator.AsposeWord
{
    public class ReplaceWithStyledTextHandler : IReplacingCallback
    {
        private readonly DocTemplateTextField _field;
        public ReplaceWithStyledTextHandler(DocTemplateTextField field) { _field = field; }

        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
        {
            Document doc = (Document)e.MatchNode.Document;

            var (firstRun, runs, indexInFirstRun) = ReplaceWithHandler.CollectAndRemoveMatch(doc.Range, e);

            if (firstRun == null || indexInFirstRun < 0)
            {
                // fallback: remove and write with preserved style
                var fallbackBuilder = new DocumentBuilder(doc);
                fallbackBuilder.MoveTo(e.MatchNode);
                ApplyFont(fallbackBuilder.Font, _field.Style, (e.MatchNode as Run)?.Font);
                e.MatchNode.Remove();
                fallbackBuilder.Write(_field.Value ?? string.Empty);
                return ReplaceAction.Skip;
            }

            // Create new run with text
            Run newRun = new(doc, _field.Value ?? string.Empty);

            // Apply placeholder style first, then apply overrides
            ApplyFont(newRun.Font, _field.Style, firstRun.Font);

            // Insert run at the right position
            if (indexInFirstRun > 0)
                firstRun.ParentNode.InsertAfter(newRun, firstRun);
            else
                firstRun.ParentNode.InsertBefore(newRun, firstRun);

            // cleanup empty runs
            foreach (var run in runs.ToList())
            {
                if (string.IsNullOrEmpty(run.Text))
                    run.Remove();
            }

            return ReplaceAction.Skip;
        }

        /// <summary>
        /// Copies style from placeholder font, then applies overrides from TemplateField.
        /// </summary>
        private static void ApplyFont(Aspose.Words.Font targetFont, DocTextStyle? f, Aspose.Words.Font? placeholderFont)
        {
            if (placeholderFont != null)
            {
                // Copy base style from placeholder
                targetFont.Name = placeholderFont.Name;
                targetFont.Size = placeholderFont.Size;
                targetFont.Bold = placeholderFont.Bold;
                targetFont.Italic = placeholderFont.Italic;
                targetFont.Color = placeholderFont.Color;
            }

            if (f is null) return;

            targetFont.Name = string.IsNullOrWhiteSpace(f.FontName) ? targetFont.Name : f.FontName;
            targetFont.Size = f.FontSize ?? targetFont.Size;
            targetFont.Bold = f.Bold ?? targetFont.Bold;
            targetFont.Italic = f.Italic ?? targetFont.Italic;
            targetFont.Color = !string.IsNullOrWhiteSpace(f.ColorHex)
                   ? ColorTranslator.FromHtml(f.ColorHex)
                   : targetFont.Color;
        }
    }
}
