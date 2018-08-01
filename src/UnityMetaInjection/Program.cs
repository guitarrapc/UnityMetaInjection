using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace UnityMetaInjection
{
    [Command(Name = "UnityMetaInjection", Description = "A very simple Unity Meta Injection")]
    [HelpOption("-h|--help|-?")]
    class Program
    {
        static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        [Option(CommandOptionType.SingleValue, Template = "-p|--path", Description = "The Unity YAML Path to inject.")]
        [FileExists]
        public string Path { get; }

        [Option(CommandOptionType.MultipleValue, Template = "-k|--kv", Description = "`Key:Value` yaml map pair to Inject")]
        public string[] KeyValues { get; set; }

        private static readonly string keyValueSeparator = ":";

        private async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            try
            {
                // Validate Path
                if (string.IsNullOrEmpty(Path))
                {
                    LogInfo("Please specify File path to inject.");
                    app.ShowHelp();
                    return 0;
                }

                // Validate KeyValue
                if (KeyValues == null || KeyValues.Length == 0)
                {
                    LogInfo("Please specify KeyValue to inject.");
                    app.ShowHelp();
                    return 0;
                }

                // Drop unexpected KeyValue argument
                var candidates = KeyValues.Where(x => x.Split(keyValueSeparator, StringSplitOptions.RemoveEmptyEntries).Length == 2).ToArray();
                if (!candidates.Any())
                {
                    LogInfo("Please specify KeyValue with `:` separated key value.");
                    app.ShowHelp();
                    return 1;
                }
                else
                {
                    // Show Dropped KeyValue
                    var dropped = KeyValues.Except(candidates).ToArray();
                    if (dropped.Any())
                    {
                        LogInfo($"Dropped unexpected keyvalue style input. [Dropped KeyValue] {string.Join(", ", dropped)}");
                    }
                }

                // Execution Plan
                LogInfo($"[ExecutePlan] -p {Path} -k {string.Join(" -k ", candidates)}");

                // Prepare
                IInjection unityMeta = new UnityMetaInject(Path, new System.Text.UTF8Encoding(false));
                foreach (var candidate in candidates)
                {
                    var kv = candidate.Split(keyValueSeparator, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    unityMeta.AddOrSet(kv[0], kv[1]);
                }

                // Execute
                LogInfo($"[BeginInjection] {string.Join(", ", unityMeta.InjectionItems.Select(x => $"Key : {x.Key}, Value : {x.Value}"))}");
                unityMeta.Inject();

                // Result
                if (!unityMeta.Validate())
                {
                    LogError("[Unexpected] Injected but unexpected result detected.");

                    // TODO : ROLLBACK (need memonize before)
                }

                return unityMeta.Validate() ? 0 : 1;
            }
            catch (Exception ex)
            {
                LogError($"{ex.GetType().FullName}, {ex.Message} {ex.StackTrace}");
                return 1;
            }
        }

        private void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"INFO: {message}");
            Console.ResetColor();
        }

        private void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }
    }
}
