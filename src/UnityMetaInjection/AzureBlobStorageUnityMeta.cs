using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnityMetaInjection
{
    public class AzureBlobStorageUnityMeta : IInjection
    {
        public string Yaml { get; }
        public Encoding Encoding { get; }
        public IDictionary<string, INode> InjectionItems { get; } = new Dictionary<string, INode>();

        public AzureBlobStorageUnityMeta(string yaml, Encoding encoding)
        {
            Yaml = yaml;
            Encoding = encoding;
        }

        public void Inject()
        {
            var keys = InjectionItems.Select(item => (item.Key, key: $"{new string(' ', item.Value.YamlIntendCount)}{item.Value.YamlKey}:", node: item.Value));
            if (IsLastlineEmpty())
            {
                RemoveLastNewLine();
            }
            var inject = File.ReadLines(Yaml)
                .Select(x =>
                {
                    foreach (var key in keys)
                    {
                        if (x.StartsWith(key.key))
                        {
                            return $"{key.key} {key.node.YamlValue}";
                        }
                    }
                    return x;
                })
                .ToArray();
            File.WriteAllText(Yaml, string.Join("\n", inject), Encoding);
        }

        public void AddOrSet(string key, INode value)
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

        private INode GetInjectionValue(string key)
        {
            if (InjectionItems.TryGetValue(key, out var result))
            {
                return result;
            }
            return null;
        }
    }
}
