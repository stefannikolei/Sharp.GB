// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Sharp.GB.Memory.cart.RTC
{
    public class Clock : IClock
    {
        private long offsetSec;
        private long clockStart;
        private long latchStart;
        private bool _halt;
        private int haltSeconds;
        private int haltMinutes;
        private int haltHours;
        private int haltDays;

        public Clock()
        {
            clockStart = Environment.TickCount64;
        }

        public void Deserialize(long[] clockData)
        {
            long seconds = clockData[0];
            long minutes = clockData[1];
            long hours = clockData[2];
            long days = clockData[3];
            long daysHigh = clockData[4];
            long timestamp = clockData[10];

            this.clockStart = timestamp * 1000;
            this.offsetSec = seconds + minutes * 60 + hours * 60 * 60 + days * 24 * 60 * 60 +
                             daysHigh * 256 * 24 * 60 * 60;
        }

        public long[] Serialize()
        {
            long[] clockData = new long[11];
            Latch();
            clockData[0] = clockData[5] = GetSeconds();
            clockData[1] = clockData[6] = GetMinutes();
            clockData[2] = clockData[7] = GetHours();
            clockData[3] = clockData[8] = GetDayCounter() % 256;
            clockData[4] = clockData[9] = GetDayCounter() / 256;
            clockData[10] = latchStart / 1000;
            Unlatch();
            return clockData;
        }

        public void Unlatch()
        {
            latchStart = 0;
        }

        public void Latch()
        {
            latchStart = Environment.TickCount64;
        }

        public void SetHalt(bool halt)
        {
            if (halt && !this._halt)
            {
                Latch();
                haltSeconds = GetSeconds();
                haltMinutes = GetMinutes();
                haltHours = GetHours();
                haltDays = GetDayCounter();
                Unlatch();
            }
            else if (!halt && this._halt)
            {
                offsetSec = haltSeconds + haltMinutes * 60 + haltHours * 60 * 60 + haltDays * 60 * 60 * 24;
                clockStart = Environment.TickCount64;
            }

            this._halt = halt;
        }

        public void ClearCounterOverflow()
        {
            while (IsCounterOverflow())
            {
                offsetSec -= 60 * 60 * 24 * 512;
            }
        }

        public bool IsCounterOverflow()
        {
            return ClockTimeInSec() >= 60 * 60 * 24 * 512;
        }

        public bool IsHalt()
        {
            return _halt;
        }

        public int GetSeconds()
        {
            return (int)(ClockTimeInSec() % 60);
        }

        public int GetMinutes()
        {
            return (int)((ClockTimeInSec() % (60 * 60)) / 60);
        }

        public int GetHours()
        {
            return (int)((ClockTimeInSec() % (60 * 60 * 24)) / (60 * 60));
        }

        public int GetDayCounter()
        {
            return (int)(ClockTimeInSec() % (60 * 60 * 24 * 512) / (60 * 60 * 24));
        }

        public void SetSeconds(int seconds)
        {
            if (!_halt)
            {
                return;
            }

            haltSeconds = seconds;
        }

        public void SetMinutes(int minutes)
        {
            if (!_halt)
            {
                return;
            }

            haltMinutes = minutes;
        }

        public void SetHours(int hours)
        {
            if (!_halt)
            {
                return;
            }

            haltHours = hours;
        }

        public void SetDayCounter(int dayCounter)
        {
            if (!_halt)
            {
                return;
            }

            haltDays = dayCounter;
        }

        private long ClockTimeInSec()
        {
            long now;
            if (latchStart == 0)
            {
                now = Environment.TickCount64;
            }
            else
            {
                now = latchStart;
            }

            return (now - clockStart) / 1000 + offsetSec;
        }
    }
}
