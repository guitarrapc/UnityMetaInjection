using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public interface IInject
    {
        IDictionary<string, string> InjectionItems { get; }

        void Inject();
        bool Validate();
        void Save();
        void Rollback();
        void AddOrSetRecord(string key, string value);
    }
}
