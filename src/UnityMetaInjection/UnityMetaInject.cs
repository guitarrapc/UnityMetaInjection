using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnityMetaInjection
{
    public class UnityMetaInject : IInject, ILoggable, INorifierable
    {
        public string Path { get; }
        public Encoding Encoding { get; }
        public IDictionary<string, string> InjectionItems { get; } = new Dictionary<string, string>();
        public ILogger Logger { get; private set; }

        private Action<ChangeMessage> notifyChange;
        private Action<ChangeMessage> defaultNotifier;
        private byte[] saveData;

        public UnityMetaInject(string path, Encoding encoding)
        {
            Path = path;
            Encoding = encoding;
            Logger = new DefaultLogger();
            defaultNotifier = change => Logger?.LogInfo($"Change detected. {nameof(change.Line)}: {change.Line}, {nameof(change.Before)}: {change.Before}, {nameof(change.After)}: {change.After}");
            notifyChange = defaultNotifier;
        }

        public void BindLogger(ILogger logger) => Logger = logger;
        public void RegisterNotifier(Action<ChangeMessage> action)
        {
            notifyChange -= defaultNotifier;
            notifyChange += action;
        }
        public void DeregisterNotifier(Action<ChangeMessage> action) => notifyChange -= action;

        public void Inject()
        {
            if (IsLastlineEmpty())
            {
                RemoveLastNewLine();
            }
            var inject = File.ReadLines(Path)
                .Select(x => EvaluateLineItem(x))
                .Select((x, i) =>
                {
                    if (x.hit && x.changed && notifyChange != null)
                    {
                        notifyChange(new ChangeMessage(i, x.before, x.after));
                    }
                    return x.after;
                })
                .ToArray();
            File.WriteAllText(Path, string.Join("\n", inject), Encoding);
        }

        public bool Validate()
        {
            var changed = File.ReadLines(Path)
                .Select(x => EvaluateLineItem(x))
                .Where(x => x.hit)
                .Any(x => x.changed);
            return !changed;
        }

        public void Save()
        {
            byte[] bytes;
            using (var reader = new FileStream(Path, FileMode.Open))
            {
                bytes = new byte[reader.Length];
                reader.Read(bytes, 0, bytes.Length);
            }
            saveData = bytes;
        }

        public void Rollback()
        {
            using (var writer = new FileStream(Path, FileMode.OpenOrCreate))
            {
                writer.Write(saveData, 0, saveData.Length);
            }
        }

        public void AddOrSetRecord(string key, string value)
        {
            if (InjectionItems.TryGetValue(key, out var result))
            {
                if (result == value)
                {
                    return;
                }
                InjectionItems[key] = value;
            }
            else
            {
                InjectionItems.Add(key, value);
            }
        }

        private (string before, string after, bool hit, bool changed) EvaluateLineItem(string current)
        {
            var intend = 0;
            for (var i = 0; i < current.Length; i++)
            {
                if (current[i] != ' ') break;
                intend++;
            }
            foreach (var item in InjectionItems)
            {
                var key = $"{new string(' ', intend)}{item.Key}";
                if (current.StartsWith(key))
                {
                    var after = $"{key}: {item.Value}";
                    return (before: current, after: after, hit: true, changed: current != after);
                }
            }
            return (before: current, after: current, hit: false, changed: false);
        }

        private bool IsLastlineEmpty()
        {
            using (var reader = new FileStream(Path, FileMode.Open))
            {
                var bytes = new byte[1];
                reader.Seek(reader.Length - 1, SeekOrigin.Begin);
                var n = reader.Read(bytes, 0, 1);
                var lastLetter = Encoding.UTF8.GetString(bytes);
                // ascii : 10 == \n
                return lastLetter == "\n";
            }
        }

        private void RemoveLastNewLine()
        {
            byte[] bytes;
            using (var reader = new FileStream(Path, FileMode.Open))
            {
                bytes = new byte[reader.Length - 1];
                reader.Read(bytes, 0, bytes.Length);
            }

            notifyChange(new ChangeMessage(File.ReadLines(Path).Count(), "\\n", ""));
            using (var writer = new FileStream(Path, FileMode.OpenOrCreate))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
        }

        private string GetInjectionValue(string key)
        {
            if (InjectionItems.TryGetValue(key, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
