﻿using System;
using Sharp.GB.Memory.Interface;

namespace Sharp.GB.Gpu
{
    public class SpriteBug
    {
        public enum CorruptionType
        {
            INC_DEC,
            POP_1,
            POP_2,
            PUSH_1,
            PUSH_2,
            LD_HL
        }

        public static void CorruptOam(IAddressSpace addressSpace, CorruptionType type, int ticksInLine)
        {
            int cpuCycle = (ticksInLine + 1) / 4 + 1;

            switch (type)
            {
                case CorruptionType.INC_DEC:
                    if (cpuCycle >= 2)
                    {
                        CopyValues(addressSpace, (cpuCycle - 2) * 8 + 2, (cpuCycle - 1) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.POP_1:
                    if (cpuCycle >= 4)
                    {
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 2, (cpuCycle - 4) * 8 + 2, 8);
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 8, (cpuCycle - 4) * 8 + 0, 2);
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 2) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.POP_2:
                    if (cpuCycle >= 5)
                    {
                        CopyValues(addressSpace, (cpuCycle - 5) * 8 + 0, (cpuCycle - 2) * 8 + 0, 8);
                    }

                    break;

                case CorruptionType.PUSH_1:
                    if (cpuCycle >= 4)
                    {
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 3) * 8 + 2, 8);
                        CopyValues(addressSpace, (cpuCycle - 3) * 8 + 2, (cpuCycle - 1) * 8 + 2, 6);
                    }

                    break;

                case CorruptionType.PUSH_2:
                    if (cpuCycle >= 5)
                    {
                        CopyValues(addressSpace, (cpuCycle - 4) * 8 + 2, (cpuCycle - 3) * 8 + 2, 8);
                    }

                    break;

                case CorruptionType.LD_HL:
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
                int b = addressSpace.getByte(0xfe00 + from + i) % 0xff;
                addressSpace.setByte(0xfe00 + to + i, b);
            }
        }
    }
}