namespace Cryptography.lab1.Utils;

public static class BitUtils
{
    public static bool GetBit(byte[] data, int index, BitOrder order)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (index < 0 || index >= data.Length * 8)
            throw new ArgumentOutOfRangeException(nameof(index));

        int byteIdx = index / 8;
        int bitInByte = index % 8;
        int shift = order == BitOrder.LsbFirst ? bitInByte : 7 - bitInByte;

        return ((data[byteIdx] >> shift) & 1) == 1;
    }

    public static void SetBit(byte[] data, int index, bool value, BitOrder order)
    {
        if (data == null) throw new ArgumentNullException("data");
        if (index < 0 || index >= data.Length * 8)
            throw new ArgumentOutOfRangeException("index");

        int byteIdx = index / 8;
        int bitInByte = index % 8;
        int shift = order == BitOrder.LsbFirst ? bitInByte : 7 - bitInByte;

        if (value) data[byteIdx] |= (byte)(1 << shift);
        else data[byteIdx] &= (byte)~(1 << shift);
    }

    public static byte[] CyclicShiftLeft(byte[] data, int shift, int bitCount, BitOrder order)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (bitCount <= 0 || bitCount > data.Length * 8)
            throw new ArgumentOutOfRangeException(nameof(bitCount));
        if (shift < 0 || shift >= bitCount)
            throw new ArgumentOutOfRangeException(nameof(shift));

        byte[] result = new byte[data.Length];
        for (int i = 0; i < bitCount; i++)
        {
            int srcIndex = (i + shift) % bitCount;
            bool bit = GetBit(data, srcIndex, order);
            SetBit(result, i, bit, order);
        }
        return result;
    }
}
