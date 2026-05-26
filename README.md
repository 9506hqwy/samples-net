# Template for C# Package

This repository provides a template for C# package.

## Prepare

1. Rename sln file name, and replace `dotnet.defaultSolution` in the *.vscode/settings.json*.
2. Remove example package project and test project.

## Development

### Formatting

Format source code.

```sh
dotnet format
```

### Testing

Execute test code.

```sh
dotnet test
```

### Profiling

#### Frame Graph

Use dotnet diagnostic tools.

```sh
dotnet trace collect --format Chromium -- <Binary Path>
```

Open *xxx.chromium.json* using Chromium.

1. Open URL *chome://tracing* or *edge://tracing*.
2. Load *xxx.chromium.json*.

#### Performance Counter

Use dotnet diagnostic tools.

```sh
dotnet counters collect -- <Binary Path>
```

### Benchmark

Use [BenchmarkDotNet](https://www.nuget.org/packages/BenchmarkDotNet/) and run benchmark application.

```sh
dotnet run -c Release --project <Benchmark Project Path>
```

### Updating

Check newer depenency packages.

```sh
dotnet list package --outdated
```

Check vulnerable packages.

```sh
dotnet list package --vulnerable
```

### License Checking

TODO

### Documentation

Generate API document.

```sh
docfx doc/docfx.json
```

### Build Artifacts

Build packages.

```sh
dotnet pack -o ./artifacts
```
