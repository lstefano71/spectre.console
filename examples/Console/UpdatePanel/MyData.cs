using System;

namespace ProgressExample
{
    internal readonly struct MyData
    {
        public MyData(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; }
        public int Count { get; }
    }
}