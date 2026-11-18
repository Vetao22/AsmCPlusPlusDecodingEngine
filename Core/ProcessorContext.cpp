#pragma once
#include <cstdint>
#include <vector>
#include <cstring>

namespace AsmCPlusPlusDecodingEngine::Core {

    /// <summary>
    /// Represents the simulated CPU state, registers, flags, and memory in modern C++.
    /// </summary>
    class ProcessorContext {
    public:
        int32_t Eax = 0;
        int32_t Ebx = 0;
        int32_t Ecx = 0;
        int32_t Edx = 0;
        int32_t Esi = 0;
        int32_t Edi = 0;
        int32_t Esp = 0;
        int32_t Ebp = 0;
        
        // Zero Flag for conditional jumps (e.g., JE, JNE)
        bool ZeroFlag = false;

        // Simulated linear RAM (64 KB)
        uint8_t Memory[65536] = { 0 };

        void Reset() {
            Eax = Ebx = Ecx = Edx = Esi = Edi = Esp = Ebp = 0;
            ZeroFlag = false;
            std::memset(Memory, 0, sizeof(Memory));
        }
    };
}