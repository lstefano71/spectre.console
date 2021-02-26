using System;
using System.Collections.Generic;

namespace ProgressExample
{
    public static class DescriptionGenerator
    {
        private static readonly string[] _verbs = new[] { "Downloading", "Rerouting", "Retriculating", "Collapsing", "Folding", "Solving", "Colliding", "Measuring" };
        private static readonly string[] _nouns = new[] { "internet", "splines", "space", "capacitators", "quarks", "algorithms", "data structures", "spacetime" };

        private static readonly Random _random;
        private static readonly HashSet<string> _used;

        static DescriptionGenerator()
        {
            _random = new Random(DateTime.Now.Millisecond);
            _used = new HashSet<string>();
        }

        public static string Generate()
        {
            return _verbs[_random.Next(0, _verbs.Length)]
                   + " " + _nouns[_random.Next(0, _nouns.Length)];
        }
    }
}