using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Replacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrasturcture.DocumentGenerator.AsposeWord
{
    public class ReplaceWithImageHandler : IReplacingCallback
    {
        private readonly string? _imagePath;
        private readonly Stream? _imageStream;
        private readonly double? _width;
        private readonly double? _height;

        public ReplaceWithImageHandler(string imagePath, double? width = null, double? height = null)
        {
            _imagePath = imagePath;
            _width = width;
            _height = height;
        }
        public ReplaceWithImageHandler(Stream imageStream, double? width = null, double? height = null)
        {
            _imageStream = imageStream;
            _width = width;
            _height = height;
        }

        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
        {
            Document doc = (Document)e.MatchNode.Document;
            var (firstRun, runs, indexInFirstRun) = ReplaceWithHandler.CollectAndRemoveMatch(doc.Range, e);

            if (firstRun == null || indexInFirstRun < 0)
            {
                return HandleFallbackInsertion(e, doc);
            }

            // Insert a placeholder run then insert image at that position
            Run placeholder = new(doc, string.Empty);
            if (indexInFirstRun > 0)
                firstRun.ParentNode.InsertAfter(placeholder, firstRun);
            else
                firstRun.ParentNode.InsertBefore(placeholder, firstRun);

            var builder = new DocumentBuilder(doc);
            builder.MoveTo(placeholder);
            InsertImage(builder);

            // remove placeholder run
            placeholder.Remove();

            // cleanup empty runs
            foreach (var run in runs.ToList())
            {
                if (string.IsNullOrEmpty(run.Text))
                    run.Remove();
            }

            return ReplaceAction.Skip;
        }
        private ReplaceAction HandleFallbackInsertion(ReplacingArgs e, Document docucment)
        {
            e.MatchNode.Remove();
            var b = new DocumentBuilder(docucment);
            b.MoveTo(e.MatchNode);
            InsertImage(b);
            return ReplaceAction.Skip;
        }
        private void InsertImage(DocumentBuilder builder)
        {
            Shape shape = _imageStream != null
                ? builder.InsertImage(_imageStream)
                : builder.InsertImage(_imagePath);

            if (_width.HasValue) shape.Width = _width.Value;
            if (_height.HasValue) shape.Height = _height.Value;
        }
    }
}
