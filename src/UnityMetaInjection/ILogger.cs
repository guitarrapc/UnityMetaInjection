using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }
}
