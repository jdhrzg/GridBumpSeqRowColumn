using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace GridBumpSeqRowColumn
{
    [Command(PackageIds.MyCommand2)]
    internal sealed class MyCommand2 : BaseCommand<MyCommand2>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            if (doc != null)
            {
                SnapshotSpan span = doc.TextView.Selection.SelectedSpans[0];
                var selectedText = span.GetText();
                var formattedSelectedText = selectedText.Replace(" ", "");

                Regex regex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");

                var matches = regex.Matches(formattedSelectedText);

                foreach (Match match in matches)
                {
                    var quoteStartIdx = match.Value.IndexOf('"');
                    var quoteEndIdx = match.Value.LastIndexOf('"');

                    if (quoteStartIdx != -1 && quoteEndIdx != -1)
                    {
                        var value = match.Value.Substring((quoteStartIdx + 1), quoteEndIdx - (quoteStartIdx + 1));
                    }
                }
            }
        }
    }
}
