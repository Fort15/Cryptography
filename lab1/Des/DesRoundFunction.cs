using Cryptography.lab1.Interfaces;
using Cryptography.lab1.Utils;

namespace Cryptography.lab1.Des;

public class DesRoundFunction : IRoundFunction
{
    public int InputBlockSize => 4;
    public int RoundKeySize => 6;
    public int OutputBlockSize => 4;

    public byte[] Apply(byte[] block, byte[] roundKey)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (block.Length != InputBlockSize)
            throw new ArgumentException($"ќжидаетс€ блок {InputBlockSize} байт, а не {block.Length}");
        if (roundKey == null) throw new ArgumentNullException(nameof(roundKey));
        if (roundKey.Length != RoundKeySize)
            throw new ArgumentException($"ќжидаетс€ ключ {RoundKeySize} байт, а не {roundKey.Length}");

        // E(R) 32 -> 48
        byte[] expanded = BitPermuter.Permute(block, DesTables.E, BitOrder.MsbFirst, BitNumbering.OneBased);

        // E(R) xor K
        byte[] xored = ByteUtils.Xor(expanded, roundKey);

        // S таблица 48 -> 32
        byte[] sboxOutput = ApplySBoxes(xored);

        // ‘инальна€ перестановка P 32 -> 32
        return BitPermuter.Permute(sboxOutput, DesTables.P, BitOrder.MsbFirst, BitNumbering.OneBased);
    }

    private static byte[] ApplySBoxes(byte[] input48)
    {
        byte[] output = new byte[4];

        for (int box = 0; box < 8; box++)
        {
            int bitOffset = box * 6;

            int row = 0;
            if (BitUtils.GetBit(input48, bitOffset, BitOrder.MsbFirst)) row |= 2;
            if (BitUtils.GetBit(input48, bitOffset + 5, BitOrder.MsbFirst)) row |= 1;

            int col = 0;
            for (int i = 0; i < 4;  i++)
            {
                col <<= 1;
                if (BitUtils.GetBit(input48, bitOffset + 1 + i, BitOrder.MsbFirst)) col |= 1;
            }

            int value = DesTables.SBoxes[box][row, col];

            int outOffset = box * 4;
            BitUtils.SetBit(output, outOffset, (value & 8) != 0, BitOrder.MsbFirst);
            BitUtils.SetBit(output, outOffset + 1, (value & 4) != 0, BitOrder.MsbFirst);
            BitUtils.SetBit(output, outOffset + 2, (value & 2) != 0, BitOrder.MsbFirst);
            BitUtils.SetBit(output, outOffset + 3, (value & 1) != 0, BitOrder.MsbFirst);
        }

        return output;
    }
}