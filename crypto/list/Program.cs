using System.Reflection;
using System.Security.Cryptography;

AppDomain.CurrentDomain.Load(typeof(AesCng).Assembly.FullName!);
AppDomain.CurrentDomain.Load(typeof(RSACryptoServiceProvider).Assembly.FullName!);

try
{
    Work();
}
#pragma warning disable CA1031
catch (Exception e)
#pragma warning restore CA1031
{
    await Console.Error.WriteLineAsync($"{e}").ConfigureAwait(false);
}

static async void Work()
{
    var supported = new List<Type>();
    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
    {
        foreach (var type in asm.GetTypes())
        {
            if (!type.IsAbstract && type.IsPublic && IsCrypto(type))
            {
                supported.Add(type);
            }
        }
    }

    foreach (var type in supported.OrderBy(t => t.FullName))
    {
        await Console.Out
            .WriteLineAsync($"{type.FullName}\t{Error(type)}")
            .ConfigureAwait(false);
    }
}

static string Error(Type type)
{
    try
    {
        _ = Activator.CreateInstance(type);
        return string.Empty;
    }
    catch (TargetInvocationException e)
    {
        return e.InnerException!.Message;
    }
#pragma warning disable CA1031
    catch (Exception e)
#pragma warning restore CA1031
    {
        return e.Message;
    }
}

static bool IsCrypto(Type type)
{
    return
        typeof(DeriveBytes).IsAssignableFrom(type) ||
        typeof(SymmetricAlgorithm).IsAssignableFrom(type) ||
        typeof(AsymmetricAlgorithm).IsAssignableFrom(type) ||
        typeof(HashAlgorithm).IsAssignableFrom(type);
}
