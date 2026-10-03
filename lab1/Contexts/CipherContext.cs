using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Contexts;


public class CipherContext
{
    private readonly ISymmetricCipher _cipher;
    private readonly CipherMode _mode;
    private readonly PaddingMode _padding;
    private readonly byte[]? _iv;
    private readonly object[] _extraParams;

    public CipherContext(ISymmetricCipher cipher, byte[] key, CipherMode mode, PaddingMode padding, byte[]? iv = null, params object[] extraParams)
    {
        _cipher = cipher ?? throw new ArgumentNullException(nameof(cipher));
        if (key == null) throw new ArgumentNullException(nameof(key));
        _cipher.SetKey(key);

        _mode = mode;
        _padding = padding;
        _iv = iv;
        _extraParams = extraParams ?? Array.Empty<object>();

        if (RequiresIv(mode) && iv == null) throw new ArgumentException($"{mode} требует IV");
        if (iv != null && iv.Length != _cipher.BlockSize)
            throw new ArgumentException($"IV должен быть длиной {_cipher.BlockSize} байт, а не {iv.Length}");
    }

    private static bool RequiresIv(CipherMode mode) => mode != CipherMode.ECB;

    public void Encrypt(byte[] data, out byte[] result)
    {
        throw new NotImplementedException();
    }

    public void Decrypt(byte[] data, out byte[] result) 
    { 
        throw new NotImplementedException();
    }

    public void Encrypt(string inputPath, string outputPath)
    {
        throw new NotImplementedException();
    }

    public void Decrypt(string inputPath, string outputPath)
    {
        throw new NotImplementedException();
    }
}