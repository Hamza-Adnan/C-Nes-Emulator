﻿using System;

namespace NESDOTNET
{
    sealed partial class CPU_CLASS
    {
        public enum InterruptType
        {
            NMI, IRQ, RESET
        }

        private readonly uint[] _interruptHandlerOffsets = { 0xFFFA, 0xFFFE, 0xFFFC };
        private readonly bool[] _interrupts = new bool[2];

        public void Initialize()
        {
            A = 0;
            X = 0;
            Y = 0;
            SP = 0xFD;
            P = 0x24;

            PC = ReadWord(_interruptHandlerOffsets[(int) InterruptType.RESET]);
        }

        public void Reset()
        {
            SP -= 3;
            F.InterruptsDisabled = true;
        }

        public void TickFromPPU()
        {
            if (CycleNUM-- > 0) return;
            ExecuteSingleInstruction();
        }

        public void ExecuteSingleInstruction()
        {
            for (int i = 0; i < _interrupts.Length; i++)
            {
                if (_interrupts[i])
                {
                    PushWord(PC);
                    Push(P);
                    PC = ReadWord(_interruptHandlerOffsets[i]);
                    F.InterruptsDisabled = true;
                    _interrupts[i] = false;
                    return;
                }
            }

            _currentInstruction = NextByte();

            CycleNUM += _opcodeDefs[_currentInstruction].Cycles;

            ResetInstructionAddressingMode();
            // if (_numExecuted > 10000 && PC - 1 == 0xFF61)
            //  if(_emulator.Controller.debug || 0x6E00 <= PC && PC <= 0x6EEF)
            //      Console.WriteLine($"{(PC - 1).ToString("X4")}  {_currentInstruction.ToString("X2")}	{opcodeNames[_currentInstruction]}\t\t\tA:{A.ToString("X2")} X:{X.ToString("X2")} Y:{Y.ToString("X2")} P:{P.ToString("X2")} SP:{SP.ToString("X2")}");

            CpuOpcode op = _opcodes[_currentInstruction];
            if (op == null)
                throw new ArgumentException(_currentInstruction.ToString("X2"));
            op();
        }

        public void TriggerInterrupt(InterruptType type)
        {
            if (!F.InterruptsDisabled || type == InterruptType.NMI)
                _interrupts[(int)type] = true;
        }
    }
}
