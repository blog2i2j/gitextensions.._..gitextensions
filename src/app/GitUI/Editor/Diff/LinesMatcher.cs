﻿using System.Diagnostics;
using GitExtensions.Extensibility;
using ICSharpCode.TextEditor.Document;

namespace GitUI.Editor.Diff;

internal static class LinesMatcher
{
    internal static IEnumerable<(ISegment RemovedLine, ISegment AddedLine)> FindLinePairs(
        Func<ISegment, string> getText, IReadOnlyList<ISegment> removedLines, IReadOnlyList<ISegment> addedLines)
    {
        int numberOfCombinations = removedLines.Count * addedLines.Count;
        if (numberOfCombinations < 1)
        {
            yield break;
        }

        // Do not try to match more lines than usually visible at the same time, because it costs O(n^2) operations
        const int maxCombinations = 100 * 100;
        if (numberOfCombinations == 1 || numberOfCombinations > maxCombinations)
        {
            int minCount = Math.Min(removedLines.Count, addedLines.Count);
            for (int i = 0; i < minCount; ++i)
            {
                yield return (removedLines[i], addedLines[i]);
            }

            yield break;
        }

        LineData[] removed = removedLines.Select(line => new LineData(line, getText(line))).ToArray();
        LineData[] added = addedLines.Select(line => new LineData(line, getText(line))).ToArray();

        foreach ((ISegment, ISegment) linePair in FindLinePairs(removed, added))
        {
            yield return linePair;
        }
    }

    private static IEnumerable<(ISegment RemovedLine, ISegment AddedLine)> FindLinePairs(LineData[] removed, LineData[] added)
    {
        (int removedIndex, int addedIndex) = FindBestMatch(removed, added);

        if (removedIndex > 0 && addedIndex > 0)
        {
            foreach ((ISegment, ISegment) linePair in FindLinePairs(removed[0..removedIndex], added[0..addedIndex]))
            {
                yield return linePair;
            }
        }

        yield return (removed[removedIndex].Line, added[addedIndex].Line);

        ++removedIndex;
        ++addedIndex;
        if (removedIndex < removed.Length && addedIndex < added.Length)
        {
            foreach ((ISegment, ISegment) linePair in FindLinePairs(removed[removedIndex..], added[addedIndex..]))
            {
                yield return linePair;
            }
        }
    }

    private static (int RemovedIndex, int AddedIndex) FindBestMatch(LineData[] removed, LineData[] added)
    {
        // first, search longest match of trimmed lines, i.e. detect indented lines
        (LineData longestMatchingRemoved, int matchingAddedIndex)
            = removed.Select(r => (r, addedIndex: added.IndexOf(a => a.Trimmed == r.Trimmed)))
                     .MaxBy(pair => pair.addedIndex < 0 ? -1 : pair.r.Trimmed.Length);
        if (matchingAddedIndex >= 0)
        {
            return (Array.IndexOf(removed, longestMatchingRemoved), matchingAddedIndex);
        }

        // then match lines whose common words have the maximum summed-up length
        int removedMaxScoreIndex = 0;
        int addedMaxScoreIndex = 0;
        float maxScore = -1;
        foreach ((int removedIndex, int addedIndex) in GetAllCombinations(removed.Length, added.Length))
        {
            float score = GetWordMatchScore(removed[removedIndex], added[addedIndex]);
            if (maxScore < score)
            {
                maxScore = score;
                removedMaxScoreIndex = removedIndex;
                addedMaxScoreIndex = addedIndex;
                if (maxScore == 1)
                {
                    return (removedMaxScoreIndex, addedMaxScoreIndex);
                }
            }
        }

        const float insignificantWordMatchScore = 0.1f;
        return maxScore <= insignificantWordMatchScore ? (0, 0) : (removedMaxScoreIndex, addedMaxScoreIndex);

        static float GetWordMatchScore(LineData r, LineData a)
        {
            if (r.Words.Count == 0 || a.Words.Count == 0)
            {
                return -1;
            }

            return (float)r.Words.Intersect(a.Words).Sum(w => w.Length) / Math.Max(r.WordsTotalLength, a.WordsTotalLength);
        }
    }

    internal static (string? CommonWord, int StartIndexRemoved, int StartIndexAdded) FindBestMatch(string textRemoved, string textAdded)
    {
        (string Word, int StartIndex) notFound = ("", -1);
        (string Word, int StartIndex)[] wordsRemoved = GetWords(textRemoved).ToArray();
        (string Word, int StartIndex)[] wordsAdded = GetWords(textAdded).ToArray();
        (string? commonWord, int startIndexOfCommonWordAdded) = wordsAdded
            .IntersectBy(wordsRemoved.Select(SelectWord), SelectWord)
            .Union([notFound])
            .MaxBy(pair => pair.Word.Length);
        if (startIndexOfCommonWordAdded != notFound.StartIndex)
        {
            return (commonWord, wordsRemoved.First(pair => pair.Word == commonWord).StartIndex, startIndexOfCommonWordAdded);
        }

        (string Word, int StartIndex)[] subwordsRemoved = GetSubwords(wordsRemoved).ToArray();
        (commonWord, startIndexOfCommonWordAdded) = GetSubwords(wordsAdded)
            .IntersectBy(subwordsRemoved.Select(SelectWord), SelectWord)
            .Union([notFound])
            .MaxBy(pair => pair.Word.Length);
        if (startIndexOfCommonWordAdded != notFound.StartIndex)
        {
            return (commonWord, subwordsRemoved.First(pair => pair.Word == commonWord).StartIndex, startIndexOfCommonWordAdded);
        }

        return (null, 0, 0);
    }

    /// <summary>
    ///  Iterates all combinations of indices - starting with (0,0), (1,0), (0,1), (2,0), (1,1), ...
    /// </summary>
    /// <returns>an enumeration of the index pairs</returns>
    internal static IEnumerable<(int FirstIndex, int SecondIndex)> GetAllCombinations(int firstEnd, int secondEnd)
    {
        // upper left half including prinicipal diagonal
        for (int diagonalIndex = 0; diagonalIndex < firstEnd; ++diagonalIndex)
        {
            int diagonalEnd = Math.Min(diagonalIndex + 1, secondEnd);
            for (int secondIndex = 0; secondIndex < diagonalEnd; ++secondIndex)
            {
                yield return (FirstIndex: diagonalIndex - secondIndex, secondIndex);
            }
        }

        // lower right half
        for (int diagonalIndex = 1; diagonalIndex < secondEnd; ++diagonalIndex)
        {
            int diagonalEnd = Math.Min(firstEnd + diagonalIndex, secondEnd);
            for (int secondIndex = diagonalIndex; secondIndex < diagonalEnd; ++secondIndex)
            {
                yield return (FirstIndex: firstEnd - 1 + diagonalIndex - secondIndex, secondIndex);
            }
        }
    }

    internal static IEnumerable<(string Word, int StartIndex)> GetSubwords(string word)
    {
        int endIndex = word.Length;
        if (endIndex == 0)
        {
            yield break;
        }

        int startIndex = 0;
        bool previousUpper = char.IsUpper(word[0]);
        for (int index = 0; index < endIndex; ++index)
        {
            bool currentUpper = char.IsUpper(word[index]);
            if (previousUpper != currentUpper)
            {
                previousUpper = currentUpper;
                if (currentUpper)
                {
                    // emit previous word, but no single '_'
                    if (!(index == 1 && !char.IsLetterOrDigit(word[0])))
                    {
                        yield return (word[startIndex..index], startIndex);
                    }

                    startIndex = index;
                }
            }

            // end word at '_', but join preceding '_' to first word
            if (index > 0 && !char.IsLetterOrDigit(word[index]))
            {
                if (startIndex < index && char.IsLetterOrDigit(word[index - 1]))
                {
                    yield return (word[startIndex..index], startIndex);
                }

                startIndex = index + 1;
                previousUpper = true;
            }
        }

        if (startIndex < endIndex && !(endIndex == 1 && !char.IsLetterOrDigit(word[0])))
        {
            yield return (word[startIndex..endIndex], startIndex);
        }
    }

    internal static IEnumerable<(string Word, int StartIndex)> GetSubwords(IEnumerable<(string Word, int StartIndex)> words)
    {
        foreach ((string Word, int StartIndex) word in words)
        {
            foreach ((string Word, int StartIndex) subword in GetSubwords(word.Word))
            {
                yield return (subword.Word, subword.StartIndex + word.StartIndex);
            }
        }
    }

    internal static IEnumerable<(string Word, int StartIndex)> GetWords(string text) => GetWords(text, IsWordChar);

    internal static IEnumerable<(string Word, int StartIndex)> GetWords(string text, Func<char, bool> isWordChar)
    {
        int length = text.Length;
        int start = 0;
        while (true)
        {
            for (; ; ++start)
            {
                if (start >= length)
                {
                    // no (more) word found, exit function
                    yield break;
                }

                if (isWordChar(text[start]))
                {
                    break;
                }
            }

            // start of word found

            for (int end = start + 1; ; ++end)
            {
                if (end >= length || !isWordChar(text[end]))
                {
                    // word end found, yield and find next word
                    yield return (text[start..end], start);
                    start = end + 1;
                    break;
                }
            }
        }
    }

    internal static bool IsWordChar(char c) => TextUtilities.IsLetterDigitOrUnderscore(c);

    internal static string SelectWord((string Word, int StartIndex) pair) => pair.Word;

    internal static int SelectStartIndex((string Word, int StartIndex) pair) => pair.StartIndex;

    [DebuggerDisplay("{Line.Offset}: {Trimmed}")]
    private readonly struct LineData
    {
        internal ISegment Line { get; }
        internal string Full { get; }
        internal string Trimmed { get; }
        internal IReadOnlySet<string> Words { get; }
        internal int WordsTotalLength { get; }

        internal LineData(ISegment line, string text)
        {
            Line = line;
            Full = text;
            Trimmed = text.Trim();
            Words = GetWords(Trimmed).Select(SelectWord).ToHashSet();
            WordsTotalLength = Words.Sum(w => w.Length);
        }
    }
}
