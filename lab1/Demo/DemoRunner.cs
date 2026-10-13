using System.Diagnostics;
using System.Text;
using Cryptography.lab1.Contexts;
using Cryptography.lab1.Des;

namespace Cryptography.lab1.Demo;

public class DemoRunner
{
    private static readonly CipherMode[] AllModes =
    {
        CipherMode.ECB, CipherMode.CBC, CipherMode.PCBC,
        CipherMode.CFB, CipherMode.OFB, CipherMode.CTR, CipherMode.RandomDelta
    };

    private readonly byte[] _key = Encoding.UTF8.GetBytes("67526752");
    private readonly byte[] _iv = Encoding.UTF8.GetBytes("RussiaRF");

    public void RunAll()
    {
        var des = new DesCipher();

        Console.WriteLine("=== ДЕМОНСТРАЦИЯ РАБОТЫ DES ===\n");

        DemoRandomBytes(des);
        DemoFiles(des, "Demo/Files");
    }

    private void DemoRandomBytes(DesCipher des)
    {
        Console.WriteLine("--- Псевднослучайные байты ---\n");

        var rng = new Random(12345);
        byte[] data = new byte[1024];
        rng.NextBytes(data);

        foreach (var mode in AllModes)
        {
            var ctx = new CipherContext(des, _key, mode, PaddingMode.PKCS7, _iv);
            TestRoundTrip(ctx, data, $"Random bytes [{mode}]");
        }

        Console.WriteLine();
    }

    private void DemoFiles(DesCipher des, string folder)
    {
        Console.WriteLine("--- Файлы ---\n");

        if (!Directory.Exists(folder))
        {
            Console.WriteLine($"Папка {folder} не найдена");
            return;
        }

        var files = Directory.GetFiles(folder);
        if (files.Length == 0)
        {
            Console.WriteLine("Папка пуста");
            return;
        }
        foreach (var file in files)
        {
            byte[] data = File.ReadAllBytes(file);
            string name = Path.GetFileName(file);

            Console.WriteLine($"Файл {name} ({data.Length} байт)");

            foreach (var mode in AllModes)
            {
                var ctx = new CipherContext(des, _key, mode, PaddingMode.PKCS7, _iv);

                TestRoundTrip(ctx, data, $" [{mode}]");
            }

            Console.WriteLine();
        }
    }

    private bool TestRoundTrip(CipherContext ctx, byte[] data, string label)
    {
        ctx.Encrypt(data, out byte[] cipher);
        ctx.Decrypt(cipher, out byte[] decrypted);

        bool ok = data.Length == decrypted.Length;
        if (ok)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != decrypted[i]) { ok = false; break; }
            }
        }

        string status = ok ? "OK" : "FAIL";
        Console.WriteLine($"{label,-30} {status}");
        return ok;
    }
}
