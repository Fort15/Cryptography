namespace Cryptography.lab1.Interfaces;

public interface ISymmetricCipher
{
    int BlockSize { get; }
    int KeySize { get; }

    void SetKey(byte[] key);
    byte[] Encrypt(byte[] block);
    byte[] Decrypt(byte[] block);
}