using System;
using System.Collections.Generic;
using System.Linq;
using Sharp.GB.Common;

namespace Sharp.GB.Cpu
{
    public enum DataType
    {
        D8,
        D16,
        R8,
        Undefined,
    }

    public class AluFunctions
    {
        private Dictionary<FunctionKey, Func<Flags, int, int>> _functions = new();

        private Dictionary<FunctionKey, Func<Flags, int, int, int>> _biFunctions = new();

        public Func<Flags, int, int> FindAluFunction(string name, DataType argumentType)
        {
            return _functions.First(x => x.Key.Name == name && x.Key.Type1 == argumentType).Value;
        }

        public Func<Flags, int, int, int> FindAluFunction(
            string name,
            DataType arg1Type,
            DataType arg2Type
        )
        {
            return _biFunctions
                .First(x =>
                    x.Key.Name == name && x.Key.Type1 == arg1Type && x.Key.Type2 == arg2Type
                )
                .Value;
        }

        private void RegisterAluFunction(
            string name,
            DataType dataType,
            Func<Flags, int, int> function
        )
        {
            _functions.Add(new(name, dataType), function);
        }

        private void RegisterAluFunction(
            string name,
            DataType dataType1,
            DataType dataType2,
            Func<Flags, int, int, int> function
        )
        {
            _biFunctions.Add(new(name, dataType1, dataType2), function);
        }

        public AluFunctions()
        {
            RegisterAluFunction(
                "INC",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg + 1) & 0xff;
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH((arg & 0x0f) == 0x0f);
                    return result;
                }
            );
            RegisterAluFunction("INC", DataType.D16, (flags, arg) => (arg + 1) & 0xffff);
            RegisterAluFunction(
                "DEC",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg - 1) & 0xff;
                    flags.SetZ(result == 0);
                    flags.SetN(true);
                    flags.SetH((arg & 0x0f) == 0x0);
                    return result;
                }
            );
            RegisterAluFunction("DEC", DataType.D16, (flags, arg) => (arg - 1) & 0xffff);
            RegisterAluFunction(
                "ADD",
                DataType.D16,
                DataType.D16,
                (flags, arg1, arg2) =>
                {
                    flags.SetN(false);
                    flags.SetH((arg1 & 0x0fff) + (arg2 & 0x0fff) > 0x0fff);
                    flags.SetC(arg1 + arg2 > 0xffff);
                    return (arg1 + arg2) & 0xffff;
                }
            );
            RegisterAluFunction(
                "ADD",
                DataType.D16,
                DataType.R8,
                (flags, arg1, arg2) => (arg1 + arg2) & 0xffff
            );
            RegisterAluFunction(
                "ADD_SP",
                DataType.D16,
                DataType.R8,
                (flags, arg1, arg2) =>
                {
                    flags.SetZ(false);
                    flags.SetN(false);

                    int result = arg1 + arg2;
                    flags.SetC((((arg1 & 0xff) + (arg2 & 0xff)) & 0x100) != 0);
                    flags.SetH((((arg1 & 0x0f) + (arg2 & 0x0f)) & 0x10) != 0);
                    return result & 0xffff;
                }
            );
            RegisterAluFunction(
                "DAA",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = arg;
                    if (flags.IsN())
                    {
                        if (flags.IsH())
                        {
                            result = (result - 6) & 0xff;
                        }

                        if (flags.IsC())
                        {
                            result = (result - 0x60) & 0xff;
                        }
                    }
                    else
                    {
                        if (flags.IsH() || (result & 0xf) > 9)
                        {
                            result += 0x06;
                        }

                        if (flags.IsC() || result > 0x9f)
                        {
                            result += 0x60;
                        }
                    }

                    flags.SetH(false);
                    if (result > 0xff)
                    {
                        flags.SetC(true);
                    }

                    result &= 0xff;
                    flags.SetZ(result == 0);
                    return result;
                }
            );
            RegisterAluFunction(
                "CPL",
                DataType.D8,
                (flags, arg) =>
                {
                    flags.SetN(true);
                    flags.SetH(true);
                    return (~arg) & 0xff;
                }
            );
            RegisterAluFunction(
                "SCF",
                DataType.D8,
                (flags, arg) =>
                {
                    flags.SetN(false);
                    flags.SetH(false);
                    flags.SetC(true);
                    return arg;
                }
            );
            RegisterAluFunction(
                "CCF",
                DataType.D8,
                (flags, arg) =>
                {
                    flags.SetN(false);
                    flags.SetH(false);
                    flags.SetC(!flags.IsC());
                    return arg;
                }
            );
            RegisterAluFunction(
                "ADD",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    flags.SetZ(((byte1 + byte2) & 0xff) == 0);
                    flags.SetN(false);
                    flags.SetH((byte1 & 0x0f) + (byte2 & 0x0f) > 0x0f);
                    flags.SetC(byte1 + byte2 > 0xff);
                    return (byte1 + byte2) & 0xff;
                }
            );
            RegisterAluFunction(
                "ADC",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    int carry = flags.IsC() ? 1 : 0;
                    flags.SetZ(((byte1 + byte2 + carry) & 0xff) == 0);
                    flags.SetN(false);
                    flags.SetH((byte1 & 0x0f) + (byte2 & 0x0f) + carry > 0x0f);
                    flags.SetC(byte1 + byte2 + carry > 0xff);
                    return (byte1 + byte2 + carry) & 0xff;
                }
            );
            RegisterAluFunction(
                "SUB",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    flags.SetZ(((byte1 - byte2) & 0xff) == 0);
                    flags.SetN(true);
                    flags.SetH((0x0f & byte2) > (0x0f & byte1));
                    flags.SetC(byte2 > byte1);
                    return (byte1 - byte2) & 0xff;
                }
            );
            RegisterAluFunction(
                "SBC",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    int carry = flags.IsC() ? 1 : 0;
                    int res = byte1 - byte2 - carry;

                    flags.SetZ((res & 0xff) == 0);
                    flags.SetN(true);
                    flags.SetH(((byte1 ^ byte2 ^ (res & 0xff)) & (1 << 4)) != 0);
                    flags.SetC(res < 0);
                    return res & 0xff;
                }
            );
            RegisterAluFunction(
                "AND",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    int result = byte1 & byte2;
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(true);
                    flags.SetC(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "OR",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    int result = byte1 | byte2;
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    flags.SetC(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "XOR",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    int result = (byte1 ^ byte2) & 0xff;
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    flags.SetC(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "CP",
                DataType.D8,
                DataType.D8,
                (flags, byte1, byte2) =>
                {
                    flags.SetZ(((byte1 - byte2) & 0xff) == 0);
                    flags.SetN(true);
                    flags.SetH((0x0f & byte2) > (0x0f & byte1));
                    flags.SetC(byte2 > byte1);
                    return byte1;
                }
            );
            RegisterAluFunction(
                "RLC",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg << 1) & 0xff;
                    if ((arg & (1 << 7)) != 0)
                    {
                        result |= 1;
                        flags.SetC(true);
                    }
                    else
                    {
                        flags.SetC(false);
                    }

                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "RRC",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = arg >> 1;
                    if ((arg & 1) == 1)
                    {
                        result |= (1 << 7);
                        flags.SetC(true);
                    }
                    else
                    {
                        flags.SetC(false);
                    }

                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "RL",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg << 1) & 0xff;
                    result |= flags.IsC() ? 1 : 0;
                    flags.SetC((arg & (1 << 7)) != 0);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "RR",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = arg >> 1;
                    result |= flags.IsC() ? (1 << 7) : 0;
                    flags.SetC((arg & 1) != 0);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "SLA",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg << 1) & 0xff;
                    flags.SetC((arg & (1 << 7)) != 0);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "SRA",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg >> 1) | (arg & (1 << 7));
                    flags.SetC((arg & 1) != 0);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "SWAP",
                DataType.D8,
                (flags, arg) =>
                {
                    int upper = arg & 0xf0;
                    int lower = arg & 0x0f;
                    int result = (lower << 4) | (upper >> 4);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    flags.SetC(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "SRL",
                DataType.D8,
                (flags, arg) =>
                {
                    int result = (arg >> 1);
                    flags.SetC((arg & 1) != 0);
                    flags.SetZ(result == 0);
                    flags.SetN(false);
                    flags.SetH(false);
                    return result;
                }
            );
            RegisterAluFunction(
                "BIT",
                DataType.D8,
                DataType.D8,
                (flags, arg1, arg2) =>
                {
                    int bit = arg2;
                    flags.SetN(false);
                    flags.SetH(true);
                    if (bit < 8)
                    {
                        flags.SetZ(!BitUtils.GetBit(arg1, arg2));
                    }

                    return arg1;
                }
            );
            RegisterAluFunction(
                "RES",
                DataType.D8,
                DataType.D8,
                (flags, arg1, arg2) => BitUtils.ClearBit(arg1, arg2)
            );
            RegisterAluFunction(
                "SET",
                DataType.D8,
                DataType.D8,
                (flags, arg1, arg2) => BitUtils.SetBit(arg1, arg2)
            );
        }

        private class FunctionKey
        {
            public readonly string Name;

            public readonly DataType Type1;

            public readonly DataType Type2;

            public FunctionKey(string name, DataType type1, DataType type2)
            {
                Name = name;
                Type1 = type1;
                Type2 = type2;
            }

            public FunctionKey(string name, DataType type)
            {
                Name = name;
                Type1 = type;
                Type2 = DataType.Undefined;
            }

            public override bool Equals(object? o)
            {
                if (this == o)
                    return true;
                if (o == null || GetType() != o.GetType())
                    return false;

                FunctionKey that = (FunctionKey)o;

                if (!Name.Equals(that.Name))
                    return false;
                if (!Type1.Equals(that.Type1))
                    return false;
                return Type2 != null ? Type2.Equals(that.Type2) : that.Type2 == null;
            }

            public override int GetHashCode()
            {
                int result = Name.GetHashCode();
                result = 31 * result + Type1.GetHashCode();
                result = 31 * result + (Type2 != null ? Type2.GetHashCode() : 0);
                return result;
            }
        }
    }
}
