using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;

namespace GridBumpSeqRowColumn
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            //var doc = await VS.Documents.GetActiveDocumentViewAsync();
            //if (doc != null)
            //{
            //    SnapshotSpan span = doc.TextView.Selection.SelectedSpans[0];
            //    var selectedText = span.GetText();
            //    var formattedSelectedText = selectedText.Replace(" ", "");

            //    Regex regex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");

            //    var matches = regex.Matches(formattedSelectedText);

            //    foreach (Match match in matches)
            //    {
            //        var quoteStartIdx = match.Value.IndexOf('"');
            //        var quoteEndIdx = match.Value.LastIndexOf('"');

            //        if (quoteStartIdx != -1 && quoteEndIdx != -1)
            //        {
            //            var value = match.Value.Substring((quoteStartIdx + 1), quoteEndIdx - (quoteStartIdx + 1));
            //        }
            //    }
            //}


            var documentView = await VS.Documents.GetActiveDocumentViewAsync();

            string selectionText = BumpSeqUtility.GetSelectionTextFromDocumentView(documentView);

            var valuesByMatch = BumpSeqUtility.GetValuesByMatchFromSelection(selectionText, FindableGridProperty.GridRow);
            
            BumpSeqUtility.SequenceValues(ref valuesByMatch);

            BumpSeqUtility.ReplaceValuesByMatchInSelection(ref selectionText, valuesByMatch, FindableGridProperty.GridRow);

            BumpSeqUtility.ApplySelectionChangesToDocument(selectionText, documentView);
        }
    }
}
