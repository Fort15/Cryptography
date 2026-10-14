using System.Diagnostics;
using System.Text;
using Cryptography.lab1.Contexts;
using Cryptography.lab1.Des;
using Cryptography.lab1.Deal;
using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Demo;

public class DemoRunner
{
    private static readonly CipherMode[] AllModes =
    {
        CipherMode.ECB, CipherMode.CBC, CipherMode.PCBC,
        CipherMode.CFB, CipherMode.OFB, CipherMode.CTR, CipherMode.RandomDelta
    };

    private readonly byte[] _desKey = Encoding.ASCII.GetBytes("67526752");
    private readonly byte[] _desIv = Encoding.ASCII.GetBytes("RussiaRF");

    private readonly byte[] _dealKey = Convert.FromHexString("00112233445566778899AABBCCDDEEFF");
    private readonly byte[] _dealIv = Convert.FromHexString("00112233445566778899AABBCCDDEEFF");

    public void RunAll()
    {
        var des = new DesCipher();
        var deal = new DealCipher();

        Console.WriteLine("=== ЗАДАНИЕ 5: ДЕМОНСТРАЦИЯ DES ===\n");
        DemoRandomBytes(des, _desKey, _desIv, "DES");
        DemoFiles(des, _desKey, _desIv, "Demo/Files/Des", "DES");

        Console.WriteLine("\n\n=== ЗАДАНИЕ 7: ДЕМОНСТРАЦИЯ DEAL ===\n");
        DemoRandomBytes(deal, _dealKey, _dealIv, "DEAL");
        DemoFiles(deal, _dealKey, _dealIv, "Demo/Files/Deal", "DEAL");
    }

    private void DemoRandomBytes(ISymmetricCipher cipher, byte[] key, byte[] iv, string label)
    {
        Console.WriteLine($"--- Псевдослучайные байты ({label}) ---\n");

        var rng = new Random(12345);
        byte[] data = new byte[1024];
        rng.NextBytes(data);

        foreach (var mode in AllModes)
        {
            byte[]? localIv = mode == CipherMode.ECB ? null : iv;
            var ctx = new CipherContext(cipher, key, mode, PaddingMode.PKCS7, localIv);
            TestRoundTrip(ctx, data, $" [{mode}]");
        }

        Console.WriteLine();
    }

    private void DemoFiles(ISymmetricCipher cipher, byte[] key, byte[] iv, string folder, string label)
    {
        Console.WriteLine($"--- Файлы ({label}) ---\n");

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
                byte[]? localIv = mode == CipherMode.ECB ? null : iv;
                var ctx = new CipherContext(cipher, key, mode, PaddingMode.PKCS7, localIv);
                TestRoundTrip(ctx, data, $" [{mode}]");
            }

            Console.WriteLine();
        }
    }

    private bool TestRoundTrip(CipherContext ctx, byte[] data, string label)
    {
        var sw = Stopwatch.StartNew();

        ctx.Encrypt(data, out byte[] cipher);
        ctx.Decrypt(cipher, out byte[] decrypted);

        sw.Stop();

        bool ok = data.Length == decrypted.Length;
        if (ok)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != decrypted[i]) { ok = false; break; }
            }
        }

        string status = ok ? "OK" : "FAIL";
        Console.WriteLine($"{label,-30} {status}  ({sw.ElapsedMilliseconds} ms)");
        return ok;
    }
}