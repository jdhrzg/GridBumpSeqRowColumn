﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GridBumpSeqRowColumn
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

    public enum FindableGridProperty
    {
        GridRow = 0,
        GridColumn = 1,
    }

    internal class BumpSeqUtility
    {
        //TODO: Consider other formats like <Grid.Row>1</Grid.Row>
        public static Regex GridRowRegex = new Regex(@"((?i)grid\.row=""(.*?)""(?-i))");
        public static Regex GridColumnRegex = new Regex(@"((?i)grid\.column=""(.*?)""(?-i))");

        public static string GetSelectionTextFromDocumentView(DocumentView? documentView)
        {
            string selectionText = null;

            if (documentView != null)
            {
                selectionText = documentView.TextView.Selection.SelectedSpans[0].GetText();
            }

            return selectionText;
        }

        public static List<MutableKeyValuePair<string, int>> GetValuesByMatchFromSelection(string selectionText, FindableGridProperty gridProperty)
        {
            List<MutableKeyValuePair<string, int>> valuesByMatch = new List<MutableKeyValuePair<string, int>>();

            var matches = GetGridPropertyMatchesFromString(gridProperty, selectionText);
            foreach (Match match in matches)
            {
                var quoteStartIdx = match.Value.IndexOf('"');
                var quoteEndIdx = match.Value.LastIndexOf('"');

                if (quoteStartIdx != -1 && quoteEndIdx != -1)
                {
                    // TODO: Test for spaces inside the quotes Grid.Row=" 2"
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

        public static void ReplaceValuesByMatchInSelection(ref string selectionText, List<MutableKeyValuePair<string, int>> valuesByMatch, FindableGridProperty gridProperty)
        {
            var matches = GetGridPropertyMatchesFromString(gridProperty, selectionText);
            for (var i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                var quoteStartIdx = match.Value.IndexOf('"');
                var quoteEndIdx = match.Value.LastIndexOf('"');

                if (quoteStartIdx != -1 && quoteEndIdx != -1)
                {
                    var matchInsideQuoteValueStart = quoteStartIdx + 1;
                    var matchInsideQuoteValueLength = quoteEndIdx - (quoteStartIdx + 1);

                    var totalInsideQuoteValueStart = match.Index + matchInsideQuoteValueStart;

                    selectionText = selectionText.Remove(totalInsideQuoteValueStart, matchInsideQuoteValueLength).Insert(totalInsideQuoteValueStart, valuesByMatch[i].Value.ToString());
                }
            }
        }

        private static MatchCollection GetGridPropertyMatchesFromString(FindableGridProperty gridProperty, string fromString)
        {
            Regex regex = null;

            if (gridProperty == FindableGridProperty.GridRow)
                regex = GridRowRegex;
            else if (gridProperty == FindableGridProperty.GridColumn)
                regex = GridColumnRegex;

            return regex.Matches(fromString);
        }

        public static void ApplySelectionChangesToDocument(string selectionTextWithChanges, DocumentView documentView)
        {
            documentView.TextBuffer.Replace(documentView.TextView.Selection.SelectedSpans[0], selectionTextWithChanges);
        }
    }
}
