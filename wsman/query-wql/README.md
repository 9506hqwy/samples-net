# Qeury WQL

## Build

Use msbuild for .Net Framework to build.

```sh
dotnet restore
msbuild
```

## Prepare

Allow http and basic auth.

```sh
winrm set winrm/config/service @{AllowUnencrypted="true"}
winrm set winrm/config/service/Auth @{Basic="true"}
```
