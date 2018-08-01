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
            ILogger logger = new DefaultLogger();
            try
            {
                // Validate Path
                if (string.IsNullOrEmpty(Path))
                {
                    logger.LogInfo("Please specify File path to inject.");
                    app.ShowHelp();
                    return 0;
                }

                // Validate KeyValue
                if (KeyValues == null || KeyValues.Length == 0)
                {
                    logger.LogInfo("Please specify KeyValue to inject.");
                    app.ShowHelp();
                    return 0;
                }

                // Drop unexpected KeyValue argument
                var candidates = KeyValues.Where(x => x.Split(keyValueSeparator, StringSplitOptions.RemoveEmptyEntries).Length == 2).ToArray();
                if (!candidates.Any())
                {
                    logger.LogInfo($"Please specify KeyValue with `{keyValueSeparator}` separated key value.");
                    app.ShowHelp();
                    return 1;
                }
                else
                {
                    // Show Dropped KeyValue
                    var dropped = KeyValues.Except(candidates).ToArray();
                    if (dropped.Any())
                    {
                        logger.LogInfo($"Dropped unexpected keyvalue style input. [Dropped KeyValue] {string.Join(", ", dropped)}");
                    }
                }

                // Execution Plan
                logger.LogInfo($"[ExecutePlan] -p {Path} -k {string.Join(" -k ", candidates)}");

                // Prepare injection
                var inject = new UnityMetaInject(Path, new System.Text.UTF8Encoding(false));
                inject.BindLogger(logger);
                inject.RegisterNotifier(change => { logger.LogInfo($"[Injecting] Injection executed. {nameof(change.Line)}: {change.Line}, {nameof(change.Before)}: {change.Before}, {nameof(change.After)}: {change.After}"); });
                foreach (var candidate in candidates)
                {
                    var kv = candidate.Split(keyValueSeparator, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    inject.AddOrSetRecord(kv[0], kv[1]);
                }

                // Run
                return Run(inject, logger);
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.GetType().FullName}, {ex.Message} {ex.StackTrace}");
                return 1;
            }
        }

        private int Run(IInject inject, ILogger logger)
        {
            // Save current before inject
            inject.Save();

            // Execute
            logger.LogInfo($"[BeginInjection] {string.Join(", ", inject.InjectionItems.Select(x => $"Key : {x.Key}, Value : {x.Value}"))}");
            inject.Inject();
            var result = inject.Validate();

            // Check Result
            if (!result)
            {
                logger.LogError("[Unexpected] Injected but unexpected result detected. Rollbacking to before injection...");

                // RollBack
                inject.Rollback();

                logger.LogError("[Rollback] Completely rollback to before injection.");
            }

            logger.LogInfo($"[CompleteInjection] Injection {(result ? "completed" : "rollbacked")}.");
            return result ? 0 : 1;
        }
    }
}
