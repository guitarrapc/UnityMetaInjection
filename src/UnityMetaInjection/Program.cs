using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityMetaInjection
{
    class Program
    {
        private static IDictionary<string, string> yamlItems = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            var commandline = new CommandLineOption();
            if (args.Length == 0)
            {
                Console.WriteLine(commandline.Help());
                return;
            }
            var options = commandline.Parse(args);
            foreach (var option in options)
            {
            }
        }
    }
}
