using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public interface INorifierable
    {
        void RegisterNotifier(Action<ChangeMessage> action);
        void DeregisterNotifier(Action<ChangeMessage> action);
    }
}
