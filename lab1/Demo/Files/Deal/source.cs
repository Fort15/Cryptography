using Cryptography.lab1.Feistel;
using Cryptography.lab1.Interfaces;
using Cryptography.lab1.Utils;

namespace Cryptography.lab1.Des;

public class DesCipher : ISymmetricCipher
{
    private readonly FeistelNetwork _network;

    public DesCipher()
    {
        _network = new FeistelNetwork(new DesKeyExpander(), new DesRoundFunction());
    }

    public int BlockSize => _network.BlockSize;
    public int KeySize => 8;

    public void SetKey(byte[] key)
    {
        _network.SetKey(key);
    }

    public byte[] Encrypt(byte[] block)
    {
        byte[] afterIp = BitPermuter.Permute(block, DesTables.IP, BitOrder.MsbFirst, BitNumbering.OneBased);
        byte[] afterRounds = _network.EncryptBlock(afterIp);
        return BitPermuter.Permute(afterRounds, DesTables.FP, BitOrder.MsbFirst, BitNumbering.OneBased);
    }

    public byte[] Decrypt(byte[] block)
    {
        byte[] afterIp = BitPermuter.Permute(block, DesTables.IP, BitOrder.MsbFirst, BitNumbering.OneBased);
        byte[] afterRounds = _network.DecryptBlock(afterIp);
        return BitPermuter.Permute(afterRounds, DesTables.FP, BitOrder.MsbFirst, BitNumbering.OneBased);
    }
}