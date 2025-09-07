using Aspose.Words;
using Aspose.Words.Replacing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
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

        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
        {
            var doc = (Document)args.MatchNode.Document;
            var (firstRun, runs, index) = ReplaceWithHandler.CollectAndRemoveMatch(doc.Range, args);

            if (firstRun == null || index < 0)
                return InsertFallbackImage(args, doc);

            var placeholder = InsertPlaceholderRun(doc, firstRun, index);
            InsertImageAt(doc, placeholder);
            CleanupRuns(runs);

            return ReplaceAction.Skip;
        }

        private ReplaceAction InsertFallbackImage(ReplacingArgs args, Document doc)
        {
            args.MatchNode.Remove();
            var builder = new DocumentBuilder(doc);
            builder.MoveTo(args.MatchNode);
            InsertImage(builder);
            return ReplaceAction.Skip;
        }

        private static Run InsertPlaceholderRun(Document doc, Run firstRun, int index)
        {
            var placeholder = new Run(doc, string.Empty);
            if (index > 0)
                firstRun.ParentNode.InsertAfter(placeholder, firstRun);
            else
                firstRun.ParentNode.InsertBefore(placeholder, firstRun);
            return placeholder;
        }

        private void InsertImageAt(Document doc, Run placeholder)
        {
            var builder = new DocumentBuilder(doc);
            builder.MoveTo(placeholder);
            InsertImage(builder);
            placeholder.Remove();
        }

        private void InsertImage(DocumentBuilder builder)
        {
            var shape = _imageStream != null
                ? builder.InsertImage(_imageStream)
                : builder.InsertImage(_imagePath);

            if (_width.HasValue) shape.Width = _width.Value;
            if (_height.HasValue) shape.Height = _height.Value;
        }

        private static void CleanupRuns(IEnumerable<Run> runs)
        {
            foreach (var run in runs.Where(r => string.IsNullOrEmpty(r.Text)))
                run.Remove();
        }
    }

}
