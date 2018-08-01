using System;
using System.Collections.Generic;
using System.Text;

namespace UnityMetaInjection
{
    public class DefaultLogger : ILogger
    {
        public void LogInfo(string message)
        {
            LogCore(ConsoleColor.DarkGray, message);
        }

        public void LogError(string message)
        {
            LogCore(ConsoleColor.Red, message);
        }

        private void LogCore(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
