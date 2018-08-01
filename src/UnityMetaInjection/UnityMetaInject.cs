using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnityMetaInjection
{
    public class UnityMetaInject : IInjection
    {
        public string Yaml { get; }
        public Encoding Encoding { get; }
        public IDictionary<string, string> InjectionItems { get; } = new Dictionary<string, string>();
        private IDictionary<int, string> before = new Dictionary<int, string>();
        private IDictionary<int, string> after = new Dictionary<int, string>();

        public UnityMetaInject(string yaml, Encoding encoding)
        {
            Yaml = yaml;
            Encoding = encoding;
        }

        public void Inject()
        {
            if (IsLastlineEmpty())
            {
                RemoveLastNewLine();
            }
            var inject = File.ReadLines(Yaml)
                .Select(x => EvaluateLineItem(x))
                .Select(x => x.line)
                .ToArray();
            File.WriteAllText(Yaml, string.Join("\n", inject), Encoding);
        }

        public bool Validate()
        {
            var validate = File.ReadLines(Yaml)
                .Select(x => EvaluateLineItem(x))
                .Where(x => x.hit)
                .Select(x => x.result)
                .All(x => x);
            return validate;
        }

        public void AddOrSet(string key, string value)
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

        private (string line, bool hit, bool result) EvaluateLineItem(string x)
        {
            var intend = 0;
            for (var i = 0; i < x.Length; i++)
            {
                if (x[i] != ' ') break;
                intend++;
            }
            foreach (var item in InjectionItems)
            {
                var key = $"{new string(' ', intend)}{item.Key}";
                if (x.StartsWith(key))
                {
                    var newline = $"{key} {item.Value}";
                    return (line: newline, hit: true, result: x == newline);
                }
            }
            return (line: x, hit: false, result: false);
        }

        private bool IsLastlineEmpty()
        {
            using (var reader = new FileStream(Yaml, FileMode.Open))
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
            using (var reader = new FileStream(Yaml, FileMode.Open))
            {
                bytes = new byte[reader.Length - 1];
                reader.Read(bytes, 0, bytes.Length);
            }
            using (var writer = new FileStream(Yaml, FileMode.OpenOrCreate))
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
