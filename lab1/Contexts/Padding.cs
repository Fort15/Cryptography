namespace Cryptography.lab1.Contexts;

public static class Padding
{
    public static byte[] Add(byte[] data, int blockSize, PaddingMode mode)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));

        int padLen = blockSize - (data.Length % blockSize);

        if (mode == PaddingMode.Zeros && padLen == blockSize)
            return (byte[])data.Clone();

        byte[] result = new byte[data.Length + padLen];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);

        switch (mode)
        {
            case PaddingMode.Zeros:
                break;

            case PaddingMode.PKCS7:
                for (int i = 0; i < padLen; i++)
                    result[data.Length + i] = (byte)padLen;
                break;

            case PaddingMode.ANSIX923:
                result[result.Length - 1] = (byte)padLen;
                break;

            case PaddingMode.ISO10126:
                var rng = new Random();
                for (int i = 0; i < padLen - 1; i++)
                    result[data.Length + i] = (byte)rng.Next(256);
                result[result.Length - 1] = (byte)padLen;
                break;

            default:
                throw new ArgumentOutOfRangeException($"Неизвестный режим набивки");
        }

        return result;
    }

    public static byte[] Remove(byte[] data, PaddingMode mode)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length == 0) throw new ArgumentException("Пустая дата");

        switch (mode)
        {
            case PaddingMode.Zeros:
                return RemoveZeros(data);

            case PaddingMode.PKCS7:
            case PaddingMode.ANSIX923:
            case PaddingMode.ISO10126:
                return RemoveByLastByte(data);

            default:
                throw new ArgumentOutOfRangeException($"Неизвестный режим набивки");
        }
    }

    private static byte[] RemoveZeros(byte[] data)
    {
        int end = data.Length;
        while (end > 0 && data[end - 1] == 0) end--;
        return data[..end];
    }

    private static byte[] RemoveByLastByte(byte[] data)
    {
        int padLen = data[data.Length - 1];
        if (padLen == 0 | padLen > data.Length) throw new ArgumentException($"Некорректная длина набивки {padLen}");
        return data[..(data.Length - padLen)];
    }
}