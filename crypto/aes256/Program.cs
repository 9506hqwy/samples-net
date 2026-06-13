using System.Security.Cryptography;
using System.Text;

try
{
    if (args.Length < 2)
    {
        ConsoleHelp();
        return;
    }

    var op = args[0];
    var target = args[1];
    string iv;
    string key;
    if (op == "enc")
    {
        if (args.Length > 3)
        {
            CreateKeyPairFrom(args[2], args[3], out iv, out key);
        }
        else
        {
            // 指定がない場合は自動的に生成する。
            CreateKey(out iv, out key);
        }

        Encrypto(target, iv, key);
    }
    else if (op == "dec")
    {
        if (args.Length < 4)
        {
            ConsoleHelp();
            return;
        }

        iv = args[2];
        key = args[3];
        Decrypto(target, iv, key);
    }
    else
    {
        ConsoleHelp();
    }
}
#pragma warning disable CA1031
catch (Exception e)
#pragma warning restore CA1031
{
    await Console.Error.WriteLineAsync($"{e}").ConfigureAwait(false);
}

static void ConsoleHelp()
{
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("    Aes.exe <OP> <TARGET> [IV] [KEY]");
    Console.WriteLine();
}

static void Decrypto(string target, string iv, string key)
{
    using var aes = CreateCrypto();
    aes.IV = Convert.FromBase64String(iv);
    aes.Key = Convert.FromBase64String(key);

    using var mem = new MemoryStream(Convert.FromBase64String(target));
    using var cryptor = aes.CreateDecryptor();
    using var crypted = new CryptoStream(mem, cryptor, CryptoStreamMode.Read);
    using var reader = new StreamReader(crypted);

    Console.WriteLine(reader.ReadToEnd());
}

static void Encrypto(string target, string iv, string key)
{
    using var aes = CreateCrypto();
    aes.IV = Convert.FromBase64String(iv);
    aes.Key = Convert.FromBase64String(key);

    using var mem = new MemoryStream();
#pragma warning disable CA5401
    using (var cryptor = aes.CreateEncryptor())
#pragma warning restore CA5401
    {
        using var crypted = new CryptoStream(mem, cryptor, CryptoStreamMode.Write);
        using var writer = new StreamWriter(crypted);
        writer.Write(target);
        writer.Flush();
    }

    Console.WriteLine("PLAIN : {0}", target);
    Console.WriteLine("IV    : {0}", iv);
    Console.WriteLine("KEY   : {0}", key);
    Console.WriteLine(Convert.ToBase64String(mem.ToArray()));
}

static SymmetricAlgorithm CreateCrypto(int blockSize = 128, int keySize = 128)
{
    var aes = Aes.Create();
    aes.BlockSize = blockSize;
    aes.KeySize = keySize;
    aes.Mode = CipherMode.CBC;
    aes.Padding = PaddingMode.PKCS7;
    return aes;
}

static void CreateKey(
    out string iv,
    out string key,
    int blockSize = 128,
    int keySize = 128)
{
    using var aes = CreateCrypto(blockSize, keySize);
    aes.GenerateIV();
    aes.GenerateKey();
    iv = Convert.ToBase64String(aes.IV);
    key = Convert.ToBase64String(aes.Key);
}

static string CreateKeyFrom(string source, int blockSize = 128)
{
    byte[] salt = new byte[blockSize / 8];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(salt);
    }

    var bytes = Rfc2898DeriveBytes.Pbkdf2(
        Encoding.UTF32.GetBytes(source),
        salt,
        100_000,
        HashAlgorithmName.SHA256,
        blockSize / 8);

    return Convert.ToBase64String(bytes);
}

static void CreateKeyPairFrom(
    string ivSource,
    string keySource,
    out string iv,
    out string key,
    int blockSize = 128,
    int keySize = 128)
{
    iv = CreateKeyFrom(ivSource, blockSize);
    key = CreateKeyFrom(keySource, keySize);
}
