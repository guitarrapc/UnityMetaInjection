using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public interface ILoggable
    {
        void BindLogger(ILogger logger);
    }
}
