namespace Cryptography.lab1.Utils;

public static class BitPermuter
{
    public static byte[] Permute(byte[] input, int[] pbox, BitOrder order, BitNumbering numbering)
    {
        if (input == null) throw new ArgumentNullException("input");
        if (pbox == null) throw new ArgumentNullException("pbox");
        if (pbox.Length == 0) throw new ArgumentException("pbox cant be empty");

        int inputBits = input.Length * 8;
        int outputBits = pbox.Length;
        byte[] output = new byte[(outputBits + 7) / 8];

        int offset = numbering == BitNumbering.OneBased ? 1 : 0;
        int minSrc = numbering == BitNumbering.OneBased ? 1 : 0;
        int maxSrc = inputBits - 1 + offset;

        for (int i = 0; i < outputBits; i++)
        {
            int src = pbox[i] - offset;
            if (src < 0 || src >= inputBits)
                throw new ArgumentException("pbox[i] out of range");
            bool bit = BitUtils.GetBit(input, src, order);
            BitUtils.SetBit(output, i, bit, order);
        }

        return output;
    }
}