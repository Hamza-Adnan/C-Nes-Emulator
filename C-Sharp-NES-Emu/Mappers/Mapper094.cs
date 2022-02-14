﻿namespace NESDOTNET.Mappers
{
    [MapperDef(Id = 94, Description = "Senjou no Ookami")]
    class Mapper094 : UxROM
    {
        public Mapper094(EMU_MAIN emulator) : base(emulator)
        {
        }

        public override void InitializeMemoryMap(CPU_CLASS cpu)
        {
            base.InitializeMemoryMap(cpu);
    
            cpu.MapWriteHandler(0x8000, 0xFFFF, (addr, val) => _bankOffset = (val & 0x1C) << 12);
        }
    }
}
