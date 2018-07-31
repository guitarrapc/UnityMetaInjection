using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public class Node : INode
    {
        public int YamlIntendCount { get; private set; }
        public string YamlKey { get; private set; }
        public string YamlValue { get; private set; }

        public Node(int intendCount, string key, string value)
        {
            YamlIntendCount = intendCount;
            YamlKey = key;
            YamlValue = value;
        }
    }
}
