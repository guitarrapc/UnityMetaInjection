using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public interface IInjection
    {
        string Yaml { get; }
        Encoding Encoding { get; }
        IDictionary<string, INode> InjectionItems { get; }
        void Inject();
        void AddOrSet(string key, INode value);
    }
}
