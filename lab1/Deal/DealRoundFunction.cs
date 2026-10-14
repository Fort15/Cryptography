using System.Buffers.Binary;
using System.Collections.Concurrent;
using Cryptography.lab1.Des;
using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Deal;

public class DealRoundFunction : IRoundFunction
{
    private readonly ConcurrentDictionary<ulong, DesCipher> _desCache = new();

    public int InputBlockSize => 8;
    public int RoundKeySize => 8;
    public int OutputBlockSize => 8;

    public byte[] Apply(byte[] block, byte[] roundKey)
    {
        if (block == null) throw new ArgumentNullException(nameof(block));
        if (block.Length != InputBlockSize)
            throw new ArgumentException($"DEAL: ожидается блок {InputBlockSize} байт, а не {block.Length}", nameof(block));
        if (roundKey == null) throw new ArgumentNullException(nameof(roundKey));
        if (roundKey.Length != RoundKeySize)
            throw new ArgumentException($"DEAL: ожидается блок {RoundKeySize} байт, а не {roundKey.Length}", nameof(roundKey));

        ulong keyInt = BinaryPrimitives.ReadUInt64BigEndian(roundKey);

        DesCipher des = _desCache.GetOrAdd(keyInt, CreateDesForKey);

        return des.Encrypt(block);
    }

    private static DesCipher CreateDesForKey(ulong keyInt)
    {
        byte[] keyBytes = new byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(keyBytes, keyInt);

        var des = new DesCipher();
        des.SetKey(keyBytes);
        return des;
    }
}