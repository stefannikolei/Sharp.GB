using System;
using Sharp.GB.Memory.cart;

namespace Sharp.GB.Memory.Extensions
{
    public static class CartridgeTypeExtensions
    {
        public static bool IsMbc1(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "MBC1");
        }

        public static bool IsMbc2(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "MBC2");
        }

        public static bool IsMbc3(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "MBC3");
        }

        public static bool IsMbc5(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "MBC5");
        }

        public static bool IsMmm01(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "MM01");
        }

        public static bool IsRam(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "RAM");
        }

        public static bool IsSram(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "SRAM");
        }

        public static bool IsTimer(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "TIMER");
        }

        public static bool IsBattery(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "BATTERY");
        }

        public static bool IsRumble(this CartridgeType cartridgeType)
        {
            return NameContainsSegment(cartridgeType, "RUMBLE");
        }

        private static bool NameContainsSegment(CartridgeType cartridgeType, string value)
        {
            return cartridgeType.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
