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
                return Elapsed.ToMinSecsMsecs();
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
            _tests.Remove(title);
            return dur;
        }
    }

    public static class Memory
    {
        public class MemSnapshot
        {
            public long CurrentlyUsed { get; set; }
            public long BeforeOOM { get; set; }
            public long Usage { get; set; }
            public long PeakUsage { get; set; }
            public long Limit { get; set; }

            public static long operator -(MemSnapshot a, MemSnapshot b)
            {
                return a.CurrentlyUsed - b.CurrentlyUsed;
            }

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
            shot.CurrentlyUsed = (long)MemoryManager.ProcessCommittedBytes;
            shot.BeforeOOM = (long)MemoryManager.ProcessCommittedLimit;
            shot.Usage = DeviceStatus.ApplicationCurrentMemoryUsage;
            shot.PeakUsage = DeviceStatus.ApplicationPeakMemoryUsage;
            shot.Limit = DeviceStatus.ApplicationMemoryUsageLimit;
            return shot;
        }
    }

    public static class Ext
    {
        public static string ToPrettyMbString(this long bytes)
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

        public static string ToMinSecsMsecs(this TimeSpan time)
        {
            return string.Format("{0:mm\\:ss\\:fff}", time);
        }
    }
}