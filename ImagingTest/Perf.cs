using System;
using System.Collections.Generic;
using System.Diagnostics;

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
}