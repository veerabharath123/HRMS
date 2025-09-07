using Aspose.Words;
using Aspose.Words.Replacing;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    /// <summary>
    /// Implements <see cref="IReplacingCallback"/> to replace a placeholder in a Word document
    /// with an image, supporting both file paths and streams.
    /// </summary>
    public class ReplaceWithImageHandler : IReplacingCallback
    {
        private readonly string? _imagePath;
        private readonly Stream? _imageStream;
        private readonly double? _width;
        private readonly double? _height;

        /// <summary>
        /// Initializes a new instance using an image file path.
        /// </summary>
        /// <param name="imagePath">The path to the image file.</param>
        /// <param name="width">Optional width of the image.</param>
        /// <param name="height">Optional height of the image.</param>
        public ReplaceWithImageHandler(string imagePath, double? width = null, double? height = null)
        {
            _imagePath = imagePath;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance using an image stream.
        /// </summary>
        /// <param name="imageStream">The stream containing the image data.</param>
        /// <param name="width">Optional width of the image.</param>
        /// <param name="height">Optional height of the image.</param>
        public ReplaceWithImageHandler(Stream imageStream, double? width = null, double? height = null)
        {
            _imageStream = imageStream;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Replaces the matched placeholder node with the specified image.
        /// Handles fallback if match is not contained within a run.
        /// </summary>
        /// <param name="args">The replacement arguments containing the matched node and value.</param>
        /// <returns>A <see cref="ReplaceAction"/> indicating to skip further replacement.</returns>
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

        /// <summary>
        /// Inserts the image at the placeholder position if the match is not contained in a run.
        /// </summary>
        private ReplaceAction InsertFallbackImage(ReplacingArgs args, Document doc)
        {
            args.MatchNode.Remove();
            var builder = new DocumentBuilder(doc);
            builder.MoveTo(args.MatchNode);
            InsertImage(builder);
            return ReplaceAction.Skip;
        }

        /// <summary>
        /// Inserts an empty run at the correct position to serve as the placeholder for the image.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="firstRun">The first run of the matched text.</param>
        /// <param name="index">The index where the match starts in the first run.</param>
        /// <returns>The placeholder <see cref="Run"/>.</returns>
        private static Run InsertPlaceholderRun(Document doc, Run firstRun, int index)
        {
            var placeholder = new Run(doc, string.Empty);
            if (index > 0)
                firstRun.ParentNode.InsertAfter(placeholder, firstRun);
            else
                firstRun.ParentNode.InsertBefore(placeholder, firstRun);
            return placeholder;
        }

        /// <summary>
        /// Inserts the image at the specified placeholder run and removes the placeholder afterwards.
        /// </summary>
        private void InsertImageAt(Document doc, Run placeholder)
        {
            var builder = new DocumentBuilder(doc);
            builder.MoveTo(placeholder);
            InsertImage(builder);
            placeholder.Remove();
        }

        /// <summary>
        /// Inserts the image using the provided <see cref="DocumentBuilder"/> and applies dimensions if specified.
        /// </summary>
        private void InsertImage(DocumentBuilder builder)
        {
            var shape = _imageStream != null
                ? builder.InsertImage(_imageStream)
                : builder.InsertImage(_imagePath);

            if (_width.HasValue) shape.Width = _width.Value;
            if (_height.HasValue) shape.Height = _height.Value;
        }

        /// <summary>
        /// Removes empty runs that are no longer needed after replacement.
        /// </summary>
        private static void CleanupRuns(IEnumerable<Run> runs)
        {
            foreach (var run in runs.Where(r => string.IsNullOrEmpty(r.Text)))
                run.Remove();
        }
    }


}
