using Cryptography.lab1.Des;

byte[] key = Convert.FromHexString("133457799BBCDFF1");
byte[] plain = Convert.FromHexString("0123456789ABCDEF");

var des = new DesCipher();
des.SetKey(key);
byte[] cipher = des.Encrypt(plain);

Console.WriteLine($"Открытый:     {Convert.ToHexString(plain)}");
Console.WriteLine($"Зашифровано:  {Convert.ToHexString(cipher)}");
Console.WriteLine($"Ожидалось:    85E813540F0AB405");
Console.WriteLine();

byte[] decrypted = des.Decrypt(cipher);
Console.WriteLine($"Расшифровано: {Convert.ToHexString(decrypted)}");
Console.WriteLine($"Ожидалось:    0123456789ABCDEF");

Console.ReadLine();