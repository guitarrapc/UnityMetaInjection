using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public class ChangeMessage
    {
        public int Line { get; }
        public string Before { get; }
        public string After { get; }

        public ChangeMessage(int line, string before, string after)
        {
            Line = line;
            Before = before;
            After = after;
        }
    }
}
