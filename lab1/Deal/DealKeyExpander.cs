using Cryptography.lab1.Des;
using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Deal;

public class DealKeyExpander : IKeyExpander
{
    private static readonly byte[] ConstantKey = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

    private readonly DesCipher _desForKeySchedule;

    public DealKeyExpander()
    {
        _desForKeySchedule = new DesCipher();
        _desForKeySchedule.SetKey(ConstantKey);
    }

    public int KeySize => 32;
    public int Rounds => 8;

    public byte[][] ExpandKey(byte[] key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        var (rounds, keyBlocksCount) = key.Length switch
        {
            16 => (6, 2),
            24 => (6, 3),
            32 => (8, 4),
            _ => throw new ArgumentException($"DEAL: неверная длина ключа {key.Length}, допустимо 16, 24, 32 байта", nameof(key))
        };

        byte[][] k = new byte[keyBlocksCount][];
        for (int i = 0; i < keyBlocksCount; i++)
        {
            k[i] = new byte[8];
            Buffer.BlockCopy(key, i * 8, k[i], 0, 8);
        }

        byte[][] roundKeys = new byte[rounds][];
        byte[] block = new byte[8];

        for (int i = 0; i < rounds; i++)
        {
            byte[] ki = k[i % keyBlocksCount];

            if (i == 0)
            {
                Buffer.BlockCopy(ki, 0, block, 0, 8);
            }
            else
            {
                for (int b = 0; b < 8; b++)
                    block[b] = (byte)(ki[b] ^ roundKeys[i - 1][b]);

                if (i >= keyBlocksCount)
                {
                    byte constant = (byte)(1 << (i - keyBlocksCount));
                    block[7] ^= constant;
                }
            }

            roundKeys[i] = _desForKeySchedule.Encrypt(block);
        }

        return roundKeys;
    }
}