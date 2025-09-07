using Aspose.Words;
using Aspose.Words.Replacing;
using System.Text;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    /// <summary>
    /// Helper class to manage text replacement in Aspose.Words documents.
    /// Provides methods to locate runs, collect matched text across runs,
    /// and remove matched content for placeholder replacement.
    /// </summary>
    public static class ReplaceWithHandler
    {
        /// <summary>
        /// Finds the nearest ancestor <see cref="Run"/> node of the specified <paramref name="node"/>.
        /// </summary>
        /// <param name="node">The node to start searching from.</param>
        /// <returns>The <see cref="Run"/> ancestor or <c>null</c> if not found.</returns>
        public static Run? GetRunAncestor(Node? node)
        {
            while (node != null && node.NodeType != NodeType.Run)
                node = node.ParentNode;
            return node as Run;
        }

        /// <summary>
        /// Collects all consecutive runs starting from the first run until the full match is found,
        /// removes the matched text from these runs, and returns information about the match.
        /// </summary>
        /// <param name="range">The document range to search in (not used currently, reserved for future).</param>
        /// <param name="args">The <see cref="ReplacingArgs"/> containing the match node and value.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><c>FirstRun</c>: The first <see cref="Run"/> containing part of the match.</item>
        /// <item><c>Runs</c>: The list of runs involved in the match.</item>
        /// <item><c>IndexInFirst</c>: The index within the first run where the match starts, or -1 if not found.</item>
        /// </list>
        /// </returns>
        public static (Run? FirstRun, List<Run> Runs, int IndexInFirst)
            CollectAndRemoveMatch(Aspose.Words.Range range, ReplacingArgs args)
        {
            var firstRun = GetRunAncestor(args.MatchNode);
            if (firstRun == null)
                return (null, new(), -1);

            var runs = CollectRunsUntilMatch(firstRun, args.Match.Value, out var concat);
            int index = concat.IndexOf(args.Match.Value, StringComparison.Ordinal);

            if (index < 0)
                return (firstRun, runs, -1);

            RemoveMatchFromRuns(runs, args.Match.Value, index);
            return (firstRun, runs, index);
        }

        /// <summary>
        /// Collects consecutive runs starting from <paramref name="start"/> until the match string is fully present.
        /// </summary>
        /// <param name="start">The first run to start collecting from.</param>
        /// <param name="match">The text to match across runs.</param>
        /// <param name="concat">The concatenated text from collected runs.</param>
        /// <returns>A list of runs involved in the match.</returns>
        private static List<Run> CollectRunsUntilMatch(Run start, string match, out string concat)
        {
            var runs = new List<Run>();
            var builder = new StringBuilder();
            Run? current = start;

            while (current != null)
            {
                runs.Add(current);
                builder.Append(current.Text);

                if (builder.ToString().Contains(match, StringComparison.Ordinal))
                    break;

                current = GetNextRun(current);
            }

            concat = builder.ToString();
            return runs;
        }

        /// <summary>
        /// Gets the next sibling <see cref="Run"/> of a given run.
        /// </summary>
        /// <param name="run">The current run.</param>
        /// <returns>The next <see cref="Run"/> sibling, or <c>null</c> if none exists.</returns>
        private static Run? GetNextRun(Run run)
        {
            Node? next = run.NextSibling;
            while (next != null && next.NodeType != NodeType.Run)
                next = next.NextSibling;
            return next as Run;
        }

        /// <summary>
        /// Removes the matched text from the given runs starting at <paramref name="startIndex"/>.
        /// Handles matches that span multiple runs.
        /// </summary>
        /// <param name="runs">The list of runs containing the match.</param>
        /// <param name="match">The matched text to remove.</param>
        /// <param name="startIndex">The start index within the first run.</param>
        private static void RemoveMatchFromRuns(List<Run> runs, string match, int startIndex)
        {
            int remaining = match.Length;
            int pos = startIndex;

            foreach (var run in runs)
            {
                string text = run.Text ?? string.Empty;
                int available = Math.Max(0, text.Length - pos);
                int removeCount = Math.Min(available, remaining);

                if (removeCount > 0)
                {
                    run.Text = text.Remove(pos, removeCount);
                    remaining -= removeCount;
                }

                if (remaining <= 0) break;
                pos = 0;
            }
        }
    }


}
