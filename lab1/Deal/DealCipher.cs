using Cryptography.lab1.Feistel;
using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Deal;

public class DealCipher : ISymmetricCipher
{
    private readonly FeistelNetwork _network;

    public DealCipher()
    {
        _network = new FeistelNetwork(new DealKeyExpander(), new DealRoundFunction());
    }

    public int BlockSize => _network.BlockSize;
    public int KeySize => 32;
    public void SetKey(byte[] key) => _network.SetKey(key);
    public byte[] Encrypt(byte[] block) => _network.EncryptBlock(block);
    public byte[] Decrypt(byte[] block) => _network.DecryptBlock(block);
}