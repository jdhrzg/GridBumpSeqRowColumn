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
        public class EditableKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public EditableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        public enum GetValueTypes
        {
            GridRow = 0,
            GridColumn = 1,
        }

        //TODO: Consider other formats like <Grid.Row>1</Grid.Row>
        public static Regex GridRowRegex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");
        public static Regex GridColumnRegex = new Regex(@"((?i)grid\.column=""(.*?)""(?-i))");

        public static async Task<List<EditableKeyValuePair<string, int>>> GetValuesByMatchFromSelectionAsync(GetValueTypes findValueType)
        {
            var valuesByMatch = new List<EditableKeyValuePair<string, int>>();

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
                            valuesByMatch.Add(new EditableKeyValuePair<string, int>(match.Value, intValue));
                        }
                    }
                }
            }

            return valuesByMatch;
        }

        public static void IncrementValues(ref List<EditableKeyValuePair<string, int>> valuesByMatch)
        {
            foreach (var valueByMatch in valuesByMatch)
            {
                valueByMatch.Value++;
            }
        }

        public static void DecrementValues(ref List<EditableKeyValuePair<string, int>> valuesByMatch)
        {
            foreach (var valueByMatch in valuesByMatch)
            {
                valueByMatch.Value--;
            }
        }


        //TODO: Left off - working on this method - get method working, after that put the values back into the selected span
        public static void SequenceValues(ref List<EditableKeyValuePair<string, int>> valuesByMatch)
        {
            var valuesGrouped = valuesByMatch.Select(x => x.Value).GroupBy(y => y);

            //foreach (var valueByMatch in valuesByMatch)
            //{ 

            //}
        }
    }
}
