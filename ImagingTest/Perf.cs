using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Phone.System.Memory;
using Microsoft.Phone.Info;

namespace ImagingTest
{
    public static class Perf
    {
        public class TestDuration
        {
            public Stopwatch Watch { get; set; }

            public string Title {get; set;}
            public DateTimeOffset StartTime { get; set; }
            public TimeSpan Elapsed {get; set;}

            public override string ToString()
            {
                return string.Format("{0}: {1}", Title, Elapsed);
            }
        }

        private static readonly Dictionary<string, TestDuration> _tests = new Dictionary<string, TestDuration>();

        public static TestDuration Checkpoint(string title)
        {
            TestDuration dur;
            if (_tests.TryGetValue(title, out dur))
            {
                dur.Elapsed = dur.Watch.Elapsed;
            }
            else
            {
                dur = new TestDuration
                    {
                        Title = title,
                        Elapsed = TimeSpan.Zero
                    };
                _tests.Add(title, dur);
                dur.StartTime = DateTimeOffset.UtcNow;
                dur.Watch = Stopwatch.StartNew();
            }
            return dur;
        }

        public static TestDuration Finish(string title)
        {
            var dur = _tests[title];
            dur.Watch.Stop();
            dur.Elapsed = dur.Watch.Elapsed;
            return dur;
        }
    }

    public static class Memory
    {
        public class MemSnapshot
        {
            public ulong CurrentlyUsed { get; set; }
            public ulong BeforeOOM { get; set; }
            public ulong Usage { get; set; }
            public ulong PeakUsage { get; set; }
            public ulong Limit { get; set; }

            public override string ToString()
            {
                return string.Format("Usage: {0}, Peak: {1}", 
                    CurrentlyUsed.ToPrettyMbString(),
                    PeakUsage.ToPrettyMbString());
            }
        }

        public static MemSnapshot Snapshot()
        {
            var shot = new MemSnapshot();
            shot.CurrentlyUsed = MemoryManager.ProcessCommittedBytes;
            shot.BeforeOOM = MemoryManager.ProcessCommittedLimit;
            shot.Usage = (ulong)DeviceStatus.ApplicationCurrentMemoryUsage;
            shot.PeakUsage = (ulong)DeviceStatus.ApplicationPeakMemoryUsage;
            shot.Limit = (ulong)DeviceStatus.ApplicationMemoryUsageLimit;
            return shot;
        }
    }

    public static class Ext
    {
        public static string ToPrettyMbString(this ulong bytes)
        {
            const int k = 1024;
            const string format = "{0:#.00} {1}";
            if (bytes < k)
                return string.Format(format, bytes, "B");
            var kilos = Math.Round((double)bytes/k);
            if (kilos < k)
                return string.Format(format, kilos, "Kb");
            return string.Format(format, kilos/k, "Mb");
        }
    }
}