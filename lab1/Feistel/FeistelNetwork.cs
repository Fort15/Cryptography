using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Feistel;

public class FeistelNetwork
{
    private readonly IKeyExpander _keyExpander;
    private readonly IRoundFunction _roundFunction;
    private byte[][]? _roundKeys;

    public FeistelNetwork(IKeyExpander keyExpander, IRoundFunction roundFunction)
    {
        _keyExpander = keyExpander;
        _roundFunction = roundFunction;
    }

    public int BlockSize => _roundFunction.InputBlockSize * 2;

    public void SetKey(byte[] key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        _roundKeys = _keyExpander.ExpandKey(key);
    }

    public byte[] EncryptBlock(byte[] block)
    {
        EnsureKeySet();
        EnsureBlockSize(block);
        return RunFeistel(block, _roundKeys!, reverse: false);
    }

    public byte[] DecryptBlock(byte[] block)
    {
        EnsureKeySet();
        EnsureBlockSize(block);
        return RunFeistel(block, _roundKeys!, reverse: true);
    }

    private byte[] RunFeistel(byte[] block, byte[][] roundKeys, bool reverse)
    {
        int halfLen = block.Length / 2;

        byte[] left = block[..halfLen];
        byte[] right = block[halfLen..];

        int rounds = roundKeys.Length;

        for (int step = 0; step < rounds; step++)
        {
            int keyIndex = reverse ? rounds - 1 - step : step;
            byte[] roundKey = _roundKeys[keyIndex];

            // F(R, K)
            byte[] fResult = _roundFunction.Apply(right, roundKey);

            // L = R, R = L xor F(R, K)
            byte[] newLeft = right;
            byte[] newRight = Xor(left, fResult);

            left = newLeft;
            right = newRight;
        }

        return Concat(right, left);
    }

    private static byte[] Xor(byte[] a, byte[] b)
    {
        if (a.Length != b.Length) throw new ArgumentException("XOR: Разные длины массивов");
        byte[] result = new byte[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = (byte)(a[i] ^ b[i]);
        }
        return result;
    }

    private static byte[] Concat(byte[] first, byte[] second)
    {
        byte[] result = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, result, 0, first.Length);
        Buffer.BlockCopy(second, 0, result,first.Length, second.Length);
        return result;
    }

    private void EnsureKeySet()
    {
        if (_roundKeys == null) throw new Exception("Сначала надо сгенерировать ключи");
    }

    private void EnsureBlockSize(byte[] block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (block.Length != BlockSize)
            throw new ArgumentException($"Нужен блокс {BlockSize} байт, а не {block.Length}");
    }
}