## UnityMetaInjection

Inject specific key:value on Unity Yaml.

## Motivation:thought_balloon:

Want to remove secrets from GitHub Source Management, but this led CI could not bundle with secret.
This tool inject to Unity YAML with commandline, so you can input Value to Unity's Scritable Object on CI or any.

## Usage:question:

```shell
$ dotnet UnityMetaInjection.dll -h
A very simple Unity Meta Injection

Usage: UnityMetaInjection [options]

Options:
  -h|--help|-?  Show help information
  -p|--path     The Unity YAML Path to inject.
  -k|--kv       `Key:Value` yaml map pair to Inject
```

## Arguments:turtle:

| Parameter | Required | Description | Usage | Tips |
| ---- | ---- | ---- | ---- | ---- | 
| -p | true | Path to the exising YAML | -p `<PATH TO THE YAML>` | YAML must exists. | 
| -k | true | `:` separated KeyValue pair to inject. | -k hoge:fuga -k piyo:poyo | Only matched section will be replace. | 

## Sample Usage:eyes:

Inject to following Uniy's Scripable Object's Value.

```meta
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12345678, guid: 12345678901234567890123456789aaa, type: 3}
  m_Name: AzureBlobConfiguration
  m_EditorClassIdentifier:
  storageAccount: 
  accessKey: 
  container: 
```

Inject command will be like this.

```shell
dotnet UnityMetaInjection.dll -p "C:\workspace\UnitySample\AzureSettings.asset" -k storageAccount:STORAGE_ACCOUNT -k accessKey:YOUR_STORAGE_KEY -k container:CONTAINER
```

Changed.

```shell
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 12345678, guid: 12345678901234567890123456789aaa, type: 3}
  m_Name: AzureBlobConfiguration
  m_EditorClassIdentifier:
  storageAccount: STORAGE_ACCOUNT
  accessKey: YOUR_STORAGE_KEY
  container: CONTAINER
  ```