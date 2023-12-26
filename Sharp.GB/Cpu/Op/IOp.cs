using Sharp.GB.Gpu;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Cpu.Op
{
    public interface IOp
    {
        bool ReadsMemory() => false;
        bool WritesMemory() => false;
        int OperandLength() => 0;
        int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context) => context;

        void SwitchInterrupts(InterruptManager interruptManager)
        {
        }

        bool Proceed(Registers registers) => true;
        bool ForceFinishCycle() => false;
        SpriteBug.CorruptionType CausesOemBug(Registers registers, int context) => default;

        static bool inOamArea(int address)
        {
            return address >= 0xfe00 && address <= 0xfeff;
        }
    }
}
