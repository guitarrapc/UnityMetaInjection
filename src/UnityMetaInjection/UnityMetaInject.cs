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
                .Select(x =>
                {
                    foreach (var item in InjectionItems)
                    {
                        var intend = 0;
                        for (var i = 0; i < x.Length; i++)
                        {
                            if (x[i] != ' ') break;
                            intend++;
                        }

                        var key = $"{new string(' ', intend)}{item.Key}";
                        if (x.StartsWith(key))
                        {
                            return $"{key} {item.Value}";
                        }
                    }
                    return x;
                })
                .ToArray();
            File.WriteAllText(Yaml, string.Join("\n", inject), Encoding);
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
