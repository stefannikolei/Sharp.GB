﻿using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class SpriteBug
    {
        public enum CorruptionType
        {
            IncDec,
            Pop1,
            Pop2,
            Push1,
            Push2,
            LdHl,
        }

        public static void CorruptOam(
            IAddressSpace addressSpace,
            CorruptionType? type,
            int ticksInLine
        )
        {
            int cpuCycle = (ticksInLine + 1) / 4 + 1;

            switch (type)
            {
                case CorruptionType.IncDec:
                    if (cpuCycle >= 2)
                    {
                        CopyValues(addressSpace, (cpuCycle - 2) * 8 + 2, (cpuCycle - 1) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.Pop1:
                    if (cpuCycle >= 4)
                    {
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 2, (cpuCycle - 4) * 8 + 2, 8);
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 8, (cpuCycle - 4) * 8 + 0, 2);
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 2) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.Pop2:
                    if (cpuCycle >= 5)
                    {
                        CopyValues(addressSpace, (cpuCycle - 5) * 8 + 0, (cpuCycle - 2) * 8 + 0, 8);
                    }

                    break;

                case CorruptionType.Push1:
                    if (cpuCycle >= 4)
                    {
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 3) * 8 + 2, 8);
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 2, (cpuCycle - 1) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.Push2:
                    if (cpuCycle >= 5)
                    {
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 3) * 8 + 2, 8);
                    }

                    break;

                case CorruptionType.LdHl:
                    if (cpuCycle >= 4)
                    {
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 2, (cpuCycle - 4) * 8 + 2, 8);
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 8, (cpuCycle - 4) * 8 + 0, 2);
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 2) * 8 + 2, 6);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void CopyValues(IAddressSpace addressSpace, int from, int to, int length)
        {
            for (int i = length - 1; i >= 0; i--)
            {
                int b = addressSpace.GetByte(0xfe00 + from + i) % 0xff;
                addressSpace.SetByte(0xfe00 + to + i, b);
            }
        }
    }
}
