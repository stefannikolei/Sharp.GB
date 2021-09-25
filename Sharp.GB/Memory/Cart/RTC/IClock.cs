// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Sharp.GB.Memory.cart.RTC
{
    public interface IClock
    {
        void Deserialize(long[] clockData);
        long[] Serialize();
        void Unlatch();
        void Latch();
        void SetHalt(bool b);
        void ClearCounterOverflow();
        bool IsCounterOverflow();
        bool IsHalt();
        int GetSeconds();
        int GetMinutes();
        int GetHours();
        int GetDayCounter();
        
        void SetSeconds(int seconds);
        void SetMinutes(int minutes);
        void SetHours(int hours);
        void SetDayCounter(int dayCounter);
    }
}
