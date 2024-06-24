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
        public enum FindValueTypes
        {
            GridRow = 0,
            GridColumn = 1,
        }

        //TODO: Consider other formats like <Grid.Row>1</Grid.Row>
        public static Regex GridRowRegex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");
        public static Regex GridColumnRegex = new Regex(@"((?i)grid\.column=""(.*?)""(?-i))");

        //TODO: Left off - working on this method, then work on doing something to those values (inc, dec, seq), after that put the values back into the selected span
        public static async List<int> GetValuesFromSelection(FindValueTypes findValueType)
        {
            var valuesFromSelection = new List<int>();

            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            if (doc != null)
            {
                string selectedText = doc.TextView.Selection.SelectedSpans[0].GetText();
                var formattedSelectedText = selectedText.Replace(" ", "");

                Regex regex = null;

                if (findValueType == FindValueTypes.GridRow)
                    regex = GridRowRegex;
                else if (findValueType == FindValueTypes.GridColumn)
                    regex = GridColumnRegex;

                var matches = regex.Matches(formattedSelectedText);

                foreach (Match match in matches)
                {
                    var quoteStartIdx = match.Value.IndexOf('"');
                    var quoteEndIdx = match.Value.LastIndexOf('"');

                    if (quoteStartIdx != -1 && quoteEndIdx != -1)
                    {
                        int intValue;

                        var success = int.TryParse(match.Value.Substring((quoteStartIdx + 1), quoteEndIdx - (quoteStartIdx + 1)), out intValue);

                        if (success)
                        {
                            valuesFromSelection.Add(intValue);
                        }
                    }
                }
            }
        }
    }
}
