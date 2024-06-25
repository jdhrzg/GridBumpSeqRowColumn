using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GridBumpSeqRowColumn
{
    internal class Shared
    {
        public enum GetValueTypes
        {
            GridRow = 0,
            GridColumn = 1,
        }

        //TODO: Consider other formats like <Grid.Row>1</Grid.Row>
        public static Regex GridRowRegex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");
        public static Regex GridColumnRegex = new Regex(@"((?i)grid\.column=""(.*?)""(?-i))");

        public static async Task<List<KeyValuePair<string, int>>> GetValuesByMatchFromSelectionAsync(GetValueTypes findValueType)
        {
            var valuesByMatchString = new List<KeyValuePair<string, int>>();

            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            if (doc != null)
            {
                string selectedText = doc.TextView.Selection.SelectedSpans[0].GetText();
                var formattedSelectedText = selectedText.Replace(" ", "");

                Regex regex = null;
                if (findValueType == GetValueTypes.GridRow)
                    regex = GridRowRegex;
                else if (findValueType == GetValueTypes.GridColumn)
                    regex = GridColumnRegex;

                var matches = regex.Matches(formattedSelectedText);
                foreach (Match match in matches)
                {
                    var quoteStartIdx = match.Value.IndexOf('"');
                    var quoteEndIdx = match.Value.LastIndexOf('"');

                    if (quoteStartIdx != -1 && quoteEndIdx != -1)
                    {
                        string stringValue = match.Value.Substring((quoteStartIdx + 1), quoteEndIdx - (quoteStartIdx + 1));
                        var parseSuccess = int.TryParse(stringValue, out int intValue);
                        if (parseSuccess)
                        {
                            valuesByMatchString.Add(new KeyValuePair<string, int>(match.Value, intValue));
                        }
                    }
                }
            }

            return valuesByMatchString;
        }

        //TODO: Left off - working on this method - need to figure out better datastructure for valuesByMatches, after that put the values back into the selected span
        public static void IncrementValues(ref List<KeyValuePair<string, int>> valuesByMatch)
        {
            foreach (var valueByMatch in valuesByMatch)
            {
                valueByMatch.Value = valueByMatch + 1;
            }
        }
    }
}
