using System.IO.Compression;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: {0} [Operation]", typeof(Program).Assembly.GetName().Name);
    Console.Error.WriteLine("");
    Console.Error.WriteLine("[Operatioin] extract [ZIP file] [Directory]");
    Console.Error.WriteLine("             archive [ZIP file] [file ...]");
    Console.Error.WriteLine("             list [ZIP file]");
    return;
}

if (string.Equals(args[0], "extract", StringComparison.OrdinalIgnoreCase) && args.Length > 2)
{
    var zipFileName = args[1];
    var destination = args[2];

    ZipFile.ExtractToDirectory(
        zipFileName,
        destination);
}
else if (string.Equals(args[0], "archive", StringComparison.OrdinalIgnoreCase) && args.Length > 2)
{
    var zipFileName = args[1];
    var elementFiles = args.Skip(2).ToArray();

    ZipFile.CreateFromDirectory(
        elementFiles.First(),
        zipFileName,
        CompressionLevel.NoCompression,
        false);
}
else if (string.Equals(args[0], "list", StringComparison.OrdinalIgnoreCase))
{
    var zipFileName = args[1];

    using var archive = ZipFile.Open(zipFileName, ZipArchiveMode.Read);

    foreach (var entry in archive.Entries)
    {
        Console.WriteLine(entry.FullName);
    }
}
else
{
    throw new NotSupportedException(string.Join(" ", args));
}

