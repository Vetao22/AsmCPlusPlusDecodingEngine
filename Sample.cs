using System;
using System.Collections.Generic;
using AsmCPlusPlusDecodingEngine.Engine;

namespace AsmCPlusPlusDecodingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample Assembly source code string simulating a loop
            string sampleAssembly = @"
                ; Initialize registers
                mov eax, 10
                mov ebx, 0

            loop_start:
                add ebx, eax
                dec eax
                cmp eax, 0
                jne loop_start
            ";

            Console.WriteLine("=== 1. Original Assembly Code ===");
            Console.WriteLine(sampleAssembly);

            // Step 1: Parse assembly lines
            var parser = new AssemblyParser();
            var instructions = parser.ParseCode(sampleAssembly, isFilePath: false);

            // Step 2: Generate C++ Source Code using the engine
            var generator = new CppCodeGenerator();
            string generatedCppCode = generator.GenerateCppSource("ExecuteCalculatedRoutine", instructions);

            Console.WriteLine("=== 2. Generated C++ Source Code ===");
            Console.WriteLine(generatedCppCode);
            
            Console.WriteLine("=== 3. Pipeline Completed Successfully ===");
            Console.WriteLine("The assembly code was parsed and translated into native C++ functions ready for compilation.");
        }
    }
}