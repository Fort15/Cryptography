using Cryptography.lab1.Contexts;
using Cryptography.lab1.Des;
using System.Text;

byte[] key = Encoding.UTF8.GetBytes("12345678");
byte[] iv = Convert.FromHexString("0011223344556677");
byte[] plain = Encoding.UTF8.GetBytes("Hello, async!");

var des = new DesCipher();
var ctx = new CipherContext(des, key, CipherMode.CBC, PaddingMode.PKCS7, iv);

// Асинхронный вызов — не блокирует
byte[] cipher = await ctx.EncryptAsync(plain);
Console.WriteLine($"Зашифровано: {Convert.ToHexString(cipher)}");

// Асинхронная расшифровка
byte[] decrypted = await ctx.DecryptAsync(cipher);
Console.WriteLine($"Расшифровано: {Encoding.UTF8.GetString(decrypted)}");
Console.WriteLine($"Совпадает: {Convert.ToHexString(plain) == Convert.ToHexString(decrypted)}");

Console.ReadLine();