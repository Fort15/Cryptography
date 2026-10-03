using Cryptography.lab1.Utils;

// Тест 1: разворот 8 бит, MSB-first, 0-based
var input = new byte[] { 0xB2, 0xFF };     // 1011 0010 1111 1111
var pBox = new[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
var result = BitPermuter.Permute(input, pBox, BitOrder.MsbFirst, BitNumbering.ZeroBased);

Console.WriteLine($"Вход:   {Convert.ToString(input[0], 2).PadLeft(8, '0')} {Convert.ToString(input[1], 2).PadLeft(8, '0')}");
Console.WriteLine($"Выход:  {Convert.ToString(result[0], 2).PadLeft(8, '0')} {Convert.ToString(result[1], 2).PadLeft(8, '0')}");
