using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GridBumpSeqRowColumn
{
    internal class Shared
    {
        public class MutableKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public MutableKeyValuePair(TKey key, TValue value)
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

        private static async Task<string> GetFormattedSelectionTextAsync()
        {
            string formattedSelectionText = null;

            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            if (doc != null)
            {
                string selectionText = doc.TextView.Selection.SelectedSpans[0].GetText();
                formattedSelectionText = selectionText.Replace(" ", "");
            }

            return formattedSelectionText;
        }

        public static async Task<List<MutableKeyValuePair<string, int>>> GetValuesByMatchFromSelectionAsync(GetValueTypes findValueType)
        {
            List<MutableKeyValuePair<string, int>> valuesByMatch = null;

            var formattedSelectionText = await GetFormattedSelectionTextAsync();

            Regex regex = null;
            if (findValueType == GetValueTypes.GridRow)
                regex = GridRowRegex;
            else if (findValueType == GetValueTypes.GridColumn)
                regex = GridColumnRegex;

            var matches = regex.Matches(formattedSelectionText);
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
                        valuesByMatch.Add(new MutableKeyValuePair<string, int>(match.Value, intValue));
                    }
                }
            }
            
            return valuesByMatch;
        }

        // TODO: LEFT 0FF - Continue on this method
        //public static async void ReplaceValuesByMatchInSelectionAsync(List<MutableKeyValuePair<string, int>> valuesByMatch)
        //{
        //    var formattedSelectionText = await GetFormattedSelectionTextAsync();


        //}

        public static void IncrementValues(ref List<MutableKeyValuePair<string, int>> valuesByMatch)
        {
            foreach (var valueByMatch in valuesByMatch)
            {
                valueByMatch.Value++;
            }
        }

        public static void DecrementValues(ref List<MutableKeyValuePair<string, int>> valuesByMatch)
        {
            foreach (var valueByMatch in valuesByMatch)
            {
                valueByMatch.Value--;
            }
        }

        public static void SequenceValues(ref List<MutableKeyValuePair<string, int>> valuesByMatch)
        {
            var valuesByMatchGrouped = valuesByMatch.GroupBy(x => x.Value);
            var newValue = valuesByMatch.Min(x => x.Value);

            foreach (var valuesByMatchGroup in valuesByMatchGrouped)
            {
                foreach (var valueByMatch in valuesByMatchGroup)
                {
                    valueByMatch.Value = newValue;
                }

                newValue++;
            }
        }
    }
}
