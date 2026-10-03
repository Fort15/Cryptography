using Cryptography.lab1.Interfaces;
using Cryptography.lab1.Utils;

namespace Cryptography.lab1.Des;


public class DesKeyExpander : IKeyExpander
{
    public int KeySize => 8;
    public int Rounds => 16;

    public byte[][] ExpandKey(byte[] key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (key.Length != KeySize)
            throw new ArgumentException($"Ключ DES должен быть {KeySize} байт, а не {key.Length}");

        // 64 -> 56
        byte[] permuted = BitPermuter.Permute(key, DesTables.G, BitOrder.MsbFirst, BitNumbering.OneBased);

        byte[] c = new byte[4];
        byte[] d = new byte[4];
        for (int i = 0; i < 28; i++)
        {
            BitUtils.SetBit(c, i, BitUtils.GetBit(permuted, i, BitOrder.MsbFirst), BitOrder.MsbFirst);
            BitUtils.SetBit(d, i, BitUtils.GetBit(permuted, i + 28, BitOrder.MsbFirst), BitOrder.MsbFirst);
        }

        byte[][] roundKeys = new byte[Rounds][];
        for (int round = 0; round < Rounds; round++)
        {
            c = BitUtils.CyclicShiftLeft(c, DesTables.Shifts[round], 28, BitOrder.MsbFirst);
            d = BitUtils.CyclicShiftLeft(d, DesTables.Shifts[round], 28, BitOrder.MsbFirst);

            byte[] cd = Concat56Bits(c, d);
            roundKeys[round] = BitPermuter.Permute(cd, DesTables.H, BitOrder.MsbFirst, BitNumbering.OneBased);
        }

        return roundKeys;
    }

    private static byte[] Concat56Bits(byte[] c, byte[] d)
    {
        byte[] result = new byte[7];
        for (int i = 0; i < 28; i++)
        {
            BitUtils.SetBit(result, i, BitUtils.GetBit(c, i, BitOrder.MsbFirst), BitOrder.MsbFirst);
            BitUtils.SetBit(result, 28 + i, BitUtils.GetBit(d, i, BitOrder.MsbFirst), BitOrder.MsbFirst);
        }
        return result;
    }
}