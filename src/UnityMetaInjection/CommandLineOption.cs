using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityMetaInjection
{
    public class CommandLineOption
    {
        public IDictionary<string, string> Parse(string[] args)
        {
            if (args.Length == 0) throw new ArgumentNullException("");
            var items = args.Where(x => x.Contains("=", StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Split("="))
                .Where(x => x.Length == 2)
                .Distinct()
                .ToDictionary(kv => kv[0], kv => kv[1]);
            return items;
        }

        public string Help()
        {
            var helpMessage = @"Please specify more than 1 arguments.
Description:
    Inject Key/Value record into specific Unity MetaData.
Usage:
    Please pass argument with `Key=Value` style.
    If you have more than 2 arguments, then write like `Key=Value,Key2=Value2`.

Syntax:
    dotnet UnityMetaInjection.dll Key1=Value1,Key2=Value2

Example:
    dotnet UnityMetaInjection.dll hoge=fuga,piyo=poyo,tyger=lion
";
            return helpMessage;
        }
    }
}
