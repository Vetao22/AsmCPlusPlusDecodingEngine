using System;
using System.Collections.Generic;
using System.Text;

namespace AsmCPlusPlusDecodingEngine.Engine
{
    /// <summary>
    /// Translates parsed assembly instructions into executable C++ class/function source code.
    /// </summary>
    public sealed class CppCodeGenerator
    {
        public string GenerateCppSource(string functionName, List<AssemblyParser.InstructionLine> instructions)
        {
            var sb = new StringBuilder();

            sb.AppendLine("#include <iostream>");
            sb.AppendLine("#include <stdexcept>");
            sb.AppendLine("#include \"ProcessorContext.h\"");
            sb.AppendLine();
            sb.AppendLine("namespace AsmCPlusPlusGenerated {");
            sb.AppendLine();
            sb.AppendLine($"    void {functionName}(AsmCPlusPlusDecodingEngine::Core::ProcessorContext& cpu)");
            sb.AppendLine("    {");
            sb.AppendLine("        try");
            sb.AppendLine("        {");

            foreach (var instr in instructions)
            {
                if (!string.IsNullOrEmpty(instr.Label))
                {
                    sb.AppendLine($"            {instr.Label}:; // Assembly Label");
                }

                if (instr.IsLabelOnly) continue;

                string translatedLine = TranslateInstruction(instr);
                if (!string.IsNullOrEmpty(translatedLine))
                {
                    sb.AppendLine($"            {translatedLine} // {instr.OriginalLine.Trim()}");
                }
                else
                {
                    sb.AppendLine($"            // TODO: Unsupported/Skipped -> {instr.OriginalLine.Trim()}");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("        catch (const std::exception& ex)");
            sb.AppendLine("        {");
            sb.AppendLine("            throw std::runtime_error(std::string(\"Runtime error during execution: \") + ex.what());");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string TranslateInstruction(AssemblyParser.InstructionLine instr)
        {
            switch (instr.Mnemonic)
            {
                case "MOV":
                    return $"{MapOperand(instr.Destination)} = {MapOperand(instr.Source)};";
                
                case "ADD":
                    return $"{MapOperand(instr.Destination)} += {MapOperand(instr.Source)};";
                
                case "SUB":
                    return $"{MapOperand(instr.Destination)} -= {MapOperand(instr.Source)};";
                
                case "INC":
                    return $"{MapOperand(instr.Destination)}++;";
                
                case "DEC":
                    return $"{MapOperand(instr.Destination)}--;";
                
                case "CMP":
                    return $"cpu.ZeroFlag = ({MapOperand(instr.Destination)} == {MapOperand(instr.Source)});";
                
                case "JMP":
                    return $"goto {instr.Destination};";
                
                case "JE":
                case "JZ":
                    return $"if (cpu.ZeroFlag) goto {instr.Destination};";
                
                case "JNE":
                case "JNZ":
                    return $"if (!cpu.ZeroFlag) goto {instr.Destination};";
                
                case "NOP":
                    return "/* NOP */;";
                
                default:
                    return null;
            }
        }

        private string MapOperand(string op)
        {
            if (string.IsNullOrEmpty(op)) return string.Empty;

            string cleanOp = op.ToUpperInvariant();

            // Map CPU registers to ProcessorContext properties in C++
            return cleanOp switch
            {
                "EAX" => "cpu.Eax",
                "EBX" => "cpu.Ebx",
                "ECX" => "cpu.Ecx",
                "EDX" => "cpu.Edx",
                "ESI" => "cpu.Esi",
                "EDI" => "cpu.Edi",
                "ESP" => "cpu.Esp",
                "EBP" => "cpu.Ebp",
                _ => op // Return literal numbers or labels as-is
            };
        }
    }
}