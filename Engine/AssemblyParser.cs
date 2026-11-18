using System;
using System.Collections.Generic;
using System.IO;

namespace AsmCPlusPlusDecodingEngine.Engine
{
    /// <summary>
    /// Parses raw assembly code lines or files into structured intermediate instruction models.
    /// </summary>
    public sealed class AssemblyParser
    {
        public sealed class InstructionLine
        {
            public string OriginalLine { get; set; }
            public string Label { get; set; }
            public string Mnemonic { get; set; }
            public string Destination { get; set; }
            public string Source { get; set; }
            public bool IsLabelOnly { get; set; }
        }

        public List<InstructionLine> ParseCode(string sourceContent, bool isFilePath = false)
        {
            string[] rawLines;
            if (isFilePath)
            {
                if (!File.Exists(sourceContent))
                    throw new FileNotFoundException($"Assembly source file not found: {sourceContent}");
                rawLines = File.ReadAllLines(sourceContent);
            }
            else
            {
                rawLines = sourceContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }

            var parsedInstructions = new List<InstructionLine>();

            foreach (var rawLine in rawLines)
            {
                string line = rawLine.Trim();

                // Strip inline comments starting with ';' or '#'
                int commentIdx = line.IndexOfAny(new[] { ';', '#' });
                if (commentIdx >= 0)
                {
                    line = line.Substring(0, commentIdx).Trim();
                }

                if (string.IsNullOrEmpty(line)) continue;

                var instr = new InstructionLine { OriginalLine = rawLine };

                // Check if line is a label (e.g., "loop_start:")
                if (line.EndsWith(":"))
                {
                    instr.Label = line.Substring(0, line.Length - 1).Trim();
                    instr.IsLabelOnly = true;
                    parsedInstructions.Add(instr);
                    continue;
                }

                // Check for inline label (e.g., "loop_start: mov eax, 0")
                if (line.Contains(":"))
                {
                    var labelParts = line.Split(new[] { ':' }, 2);
                    instr.Label = labelParts[0].Trim();
                    line = labelParts[1].Trim();
                }

                // Tokenize mnemonic and operands
                var tokens = line.Split(new[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    instr.Mnemonic = tokens[0].ToUpperInvariant();
                    if (tokens.Length > 1) instr.Destination = tokens[1];
                    if (tokens.Length > 2) instr.Source = tokens[2];
                }

                parsedInstructions.Add(instr);
            }

            return parsedInstructions;
        }
    }
}