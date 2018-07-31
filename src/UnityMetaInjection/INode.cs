namespace UnityMetaInjection
{
    public interface INode
    {
        int YamlIntendCount { get; }
        string YamlKey { get; }
        string YamlValue { get; }
    }
}