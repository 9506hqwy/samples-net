using System.Collections;
using System.Dynamic;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;

try
{
    Work();
}
#pragma warning disable CA1031
catch (Exception e)
#pragma warning restore CA1031
{
    Console.WriteLine(e);
}

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
static IEnumerable<ExpandoObject> InvokeWql(string wql)
{
    var scope = new ManagementScope(@"root\cimv2");
    var query = new SelectQuery(wql);
    using var searcher = new ManagementObjectSearcher(scope, query);
    using var collections = searcher.Get();
    foreach (var obj in collections)
    {
        var result = new ExpandoObject();
        var data = result as IDictionary<string, object>;
        foreach (var property in obj.Properties)
        {
            data.Add(property.Name, obj[property.Name]);
        }

        yield return result;
    }
}

static IEnumerable<string> EnumerateDotNetFramework()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        // https://learn.microsoft.com/ja-jp/dotnet/framework/install/how-to-determine-which-versions-are-installed
        using var bKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\")!;
        foreach (var nKey in bKey.GetSubKeyNames().Where(n => n.StartsWith('v')))
        {
            using var vKey = bKey.OpenSubKey(nKey)!;
            var install = (int)vKey.GetValue("Install", -1);
            if (install == -1)
            {
                continue;
            }

            var version = (string)vKey.GetValue("Version", string.Empty);
            var sp = (int)vKey.GetValue("SP", 0);
            yield return $"v{version} SP {sp}";
        }

        using (var vKey = bKey.OpenSubKey(@"v4\Full")!)
        {
            var install = (int)vKey.GetValue("Install", -1);
            if (install != -1)
            {
                var version = (string)vKey.GetValue("Version", string.Empty);
                var release = (int)vKey.GetValue("Release", 0);
                yield return $"v{version} Release {release}";
            }
        }
    }
}

static void Work()
{
    Console.WriteLine("Hardware:");
    Console.WriteLine("  CPU: {0}", Environment.ProcessorCount);
    if (OperatingSystem.IsWindows())
    {
        foreach (var cpu in InvokeWql("SELECT * FROM Win32_Processor"))
        {
            foreach (var d in cpu as IDictionary<string, object>)
            {
                Console.WriteLine("  CPU.{0}: {1}", d.Key, d.Value);
            }
        }
    }

    Console.WriteLine();

    Console.WriteLine("OperatingSystem:");
    Console.WriteLine("  OS: {0}", Environment.OSVersion);
    Console.WriteLine("  x64: {0}", Environment.Is64BitOperatingSystem);
    if (OperatingSystem.IsWindows())
    {
        foreach (var cpu in InvokeWql("SELECT * FROM Win32_OperatingSystem"))
        {
            foreach (var d in cpu as IDictionary<string, object>)
            {
                Console.WriteLine("  OS.{0}: {1}", d.Key, d.Value);
            }
        }
    }

    Console.WriteLine();


    Console.WriteLine("Environment:");
    foreach (var env in Environment.GetEnvironmentVariables().OfType<DictionaryEntry>())
    {
        Console.WriteLine("  Env.{0}: {1}", env.Key, env.Value);
    }

    Console.WriteLine();

    Console.WriteLine(".Net Runtime:");
    Console.WriteLine("  Version             : {0}", Environment.Version);
    Console.WriteLine("  x64                 : {0}", Environment.Is64BitProcess);
    Console.WriteLine("  Build CLR Version   : {0}", typeof(Program).Assembly.ImageRuntimeVersion);
    Console.WriteLine("  Runtime CLR Version : {0}", RuntimeEnvironment.GetSystemVersion());
    Console.WriteLine("  FrameworkDescription: {0}", RuntimeInformation.FrameworkDescription);
    Console.WriteLine("  OSArchitecture      : {0}", RuntimeInformation.OSArchitecture);
    Console.WriteLine("  OSDescription       : {0}", RuntimeInformation.OSDescription);
    Console.WriteLine("  ProcessArchitecture : {0}", RuntimeInformation.ProcessArchitecture);
    foreach (var n in EnumerateDotNetFramework())
    {
        Console.WriteLine("  .NET Framework      : {0}", n);
    }

    Console.WriteLine();
}
