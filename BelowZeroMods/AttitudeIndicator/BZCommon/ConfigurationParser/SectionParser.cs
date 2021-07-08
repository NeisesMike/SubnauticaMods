using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace BZCommon.ConfigurationParser
{
    public class SectionParser
    {
        private FileReader _reader;
        private static readonly Regex SectionPattern = new Regex(@"^\[(.*)\]$");        
        private static readonly Regex PairPattern = new Regex(@"^([\S][^:]+)[\s]*:[\s]*(.*)$");
        private static readonly Regex ContinuationPattern = new Regex(@"^[\s]+[\S]+");

        public Dictionary<string, Section> Sections { get; private set; }

        public SectionParser(FileReader reader)
        {
            _reader = reader;
            EnsureFileBeginsWithSection();
            InitSections();
        }

        private void InitSections()
        {
            Sections = new Dictionary<string, Section>();
            _reader.Lines.All(l => { ParseLine(l); return true; });
        }

        private void ParseLine(string line)
        {
            if (SectionPattern.IsMatch(line))
                InitNewSectionFromLine(line);
            if (PairPattern.IsMatch(line))
                AddKeyValuePairToLastSectionFromLine(line);
            if (ContinuationPattern.IsMatch(line))
                AppendToLastSectionValueFromLine(line);
        }

        private void InitNewSectionFromLine(string line)
        {
            var match = SectionPattern.Match(line);
            var sectionKey = match.Groups[1].Value.Trim();
            Sections.Add(sectionKey, new Section(sectionKey));
        }

        private void AddKeyValuePairToLastSectionFromLine(string line)
        {
            var match = PairPattern.Match(line);
            var key = match.Groups[1].Value.Trim();
            var value = match.Groups[2].Value;
            Sections[Sections.Last().Key].Add(key, value);
        }

        private void AppendToLastSectionValueFromLine(string line)
        {
            var sectionKey = Sections.Last().Key;

            var lastPair = Sections[sectionKey].LastOrDefault();

            if (lastPair.Equals(default(KeyValuePair<string, string>)))
            {
                throw new ArgumentException("Section contains invalid key format");
            }

            Sections[sectionKey][lastPair.Key] += line;
        }

        private void EnsureFileBeginsWithSection()
        {
            if (!SectionPattern.IsMatch(_reader.Lines[0]))
                throw new ArgumentException(string.Format("{0} - Beginning line is not a valid section heading", _reader.Lines[0]));
        }
    }
}