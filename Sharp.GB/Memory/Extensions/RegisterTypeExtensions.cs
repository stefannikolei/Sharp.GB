using System;
using Sharp.GB.Memory.Enums;

namespace Sharp.GB.Memory.Extensions
{
    public static class RegisterTypeExtensions
    {
        public static bool AllowsRead(this RegisterType registerType)
        {
            return registerType switch
            {
                RegisterType.Read => true,
                RegisterType.Write => false,
                RegisterType.ReadWrite => true,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(registerType),
                    registerType,
                    null
                ),
            };
        }

        public static bool AllowsWrite(this RegisterType registerType)
        {
            return registerType switch
            {
                RegisterType.Read => false,
                RegisterType.Write => true,
                RegisterType.ReadWrite => true,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(registerType),
                    registerType,
                    null
                ),
            };
        }
    }
}
