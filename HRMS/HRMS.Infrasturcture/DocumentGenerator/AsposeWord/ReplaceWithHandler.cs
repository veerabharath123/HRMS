using Aspose.Words;
using Aspose.Words.Replacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Infrasturcture.DocumentGenerator.AsposeWord
{
    public static class ReplaceWithHandler
    {
        /// <summary>
        /// Helper: finds the Run that contains (or is an ancestor of) the given node.
        /// </summary>
        public static Run? GetRunAncestor(Node? node)
        {
            Node? cur = node;
            while (cur != null && cur.NodeType != NodeType.Run)
                cur = cur.ParentNode;
            return cur as Run;
        }

        /// <summary>
        /// Removes the exact match text (which may span several runs) and returns the runs that were part of the concatenation.
        /// Also returns the index where the match starts relative to the first run's text.
        /// </summary>
        public static (Run? firstRun, List<Run> runs, int indexInFirstRun) CollectAndRemoveMatch(Aspose.Words.Range range, ReplacingArgs e)
        {
            string matchText = e.Match.Value;
            Run? firstRun = GetRunAncestor(e.MatchNode);
            if (firstRun == null)
                return (null, new List<Run>(), -1);

            // Build concatenation of consecutive runs starting at firstRun until we can find the match
            List<Run> runs = new List<Run>();
            StringBuilder sb = new StringBuilder();
            Run? r = firstRun;
            while (r != null)
            {
                runs.Add(r);
                sb.Append(r.Text);

                if (sb.ToString().IndexOf(matchText, StringComparison.Ordinal) >= 0)
                    break;

                // advance to next run sibling
                Node? next = r.NextSibling;
                while (next != null && next.NodeType != NodeType.Run)
                    next = next.NextSibling;
                r = next as Run;
            }

            string concat = sb.ToString();
            int indexInConcat = concat.IndexOf(matchText, StringComparison.Ordinal);
            if (indexInConcat < 0)
                return (firstRun, runs, -1);

            int indexInFirstRun = indexInConcat; // concat starts at firstRun beginning

            // Remove matched characters across the runs list
            int remaining = matchText.Length;
            int pos = indexInFirstRun;
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
                // subsequent runs: pos = 0
                pos = 0;
                if (remaining <= 0) break;
            }

            return (firstRun, runs, indexInFirstRun);
        }
    }
}
