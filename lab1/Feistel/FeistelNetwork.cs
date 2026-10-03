using Cryptography.lab1.Interfaces;
using Cryptography.lab1.Utils;

namespace Cryptography.lab1.Feistel;

public class FeistelNetwork
{
    private readonly IKeyExpander _keyExpander;
    private readonly IRoundFunction _roundFunction;
    private byte[][]? _roundKeys;

    public FeistelNetwork(IKeyExpander keyExpander, IRoundFunction roundFunction)
    {
        _keyExpander = keyExpander ?? throw new ArgumentNullException(nameof(keyExpander));
        _roundFunction = roundFunction ?? throw new ArgumentNullException(nameof(roundFunction));
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
            byte[] roundKey = roundKeys[keyIndex];

            // F(R, K)
            byte[] fResult = _roundFunction.Apply(right, roundKey);

            // L = R, R = L xor F(R, K)
            byte[] newLeft = right;
            byte[] newRight = ByteUtils.Xor(left, fResult);

            left = newLeft;
            right = newRight;
        }

        return ByteUtils.Concat(right, left);
    }

    private void EnsureKeySet()
    {
        if (_roundKeys == null) throw new Exception("Сначала надо сгенерировать ключи");
    }

    private void EnsureBlockSize(byte[] block)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (block.Length != BlockSize)
            throw new ArgumentException($"Нужен блок {BlockSize} байт, а не {block.Length}");
    }
}