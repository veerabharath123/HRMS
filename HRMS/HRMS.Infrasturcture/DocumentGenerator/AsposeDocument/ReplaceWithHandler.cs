using Aspose.Words;
using Aspose.Words.Replacing;
using System.Text;

namespace HRMS.Infrastructure.DocumentGenerator.AsposeDocument
{
    public static class ReplaceWithHandler
    {
        public static Run? GetRunAncestor(Node? node)
        {
            while (node != null && node.NodeType != NodeType.Run)
                node = node.ParentNode;
            return node as Run;
        }

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

        private static Run? GetNextRun(Run run)
        {
            Node? next = run.NextSibling;
            while (next != null && next.NodeType != NodeType.Run)
                next = next.NextSibling;
            return next as Run;
        }

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
