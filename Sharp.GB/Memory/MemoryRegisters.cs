using System;
using System.Collections.Generic;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Memory
{
    public class MemoryRegisters : IAddressSpace
    {
        private Dictionary<int, IRegister> _registers = new();
        private Dictionary<int, int> _values = new();

        public MemoryRegisters(IRegister[] registers)
        {
            foreach (var register in registers)
            {
                if (_registers.ContainsKey(register.GetAddress()))
                {
                    throw new ArgumentException(
                        "Two registers with the same address can not be registered "
                            + register.GetAddress()
                    );
                }

                _registers.Add(register.GetAddress(), register);
                _values.Add(register.GetAddress(), 0);
            }
        }

        private MemoryRegisters(MemoryRegisters original)
        {
            _registers = original._registers;
            _values = new(original._values);
        }

        public int this[IRegister register]
        {
            get
            {
                if (_registers.ContainsKey(register.GetAddress()))
                {
                    return _values[register.GetAddress()];
                }

                throw new ArgumentException("No valid register " + register);
            }
        }

        public void Put(IRegister register, int value2)
        {
            if (_registers.ContainsKey(register.GetAddress()))
            {
                _values[register.GetAddress()] = value2;
                return;
            }

            throw new ArgumentException("No valid register " + register);
        }

        public MemoryRegisters Freeze()
        {
            return new(this);
        }

        public int PreIncrement(IRegister register)
        {
            if (_registers.ContainsKey(register.GetAddress()))
            {
                var value = _values[register.GetAddress()];
                _values[register.GetAddress()] = value;
                return value;
            }

            throw new ArgumentException("No valid register " + register);
        }

        public bool Accepts(int address)
        {
            return _registers.ContainsKey(address);
        }

        public void SetByte(int address, int value)
        {
            if (_registers[address].Type.AllowsWrite)
            {
                _values[address] = value;
            }
        }

        public int GetByte(int address)
        {
            if (_registers[address].Type.AllowsRead)
            {
                return _values[address];
            }

            return 0xff;
        }
    }
}
