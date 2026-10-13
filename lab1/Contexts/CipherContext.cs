using System.Threading.Tasks;
using Cryptography.lab1.Interfaces;
using Cryptography.lab1.Utils;

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
        if (data == null) throw new ArgumentNullException(nameof(data));

        byte[] padded = Padding.Add(data, _cipher.BlockSize, _padding);

        result = _mode switch
        {
            CipherMode.ECB => EncryptEcb(padded),
            CipherMode.CBC => EncryptCbc(padded),
            CipherMode.PCBC => EncryptPcbc(padded),
            CipherMode.CFB => EncryptCfb(padded),
            CipherMode.OFB => EncryptOfb(padded),
            CipherMode.CTR => EncryptCtr(padded),
            CipherMode.RandomDelta => EncryptRd(padded),
            _ => throw new NotSupportedException()
        };
    }

    public void Decrypt(byte[] data, out byte[] result) 
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data.Length % _cipher.BlockSize != 0)
            throw new ArgumentException($"Длина данных {data.Length} не кратна размеру блока {_cipher.BlockSize}");

        byte[] decrypted = _mode switch
        {
            CipherMode.ECB => DecryptEcb(data),
            CipherMode.CBC => DecryptCbc(data),
            CipherMode.PCBC => DecryptPcbc(data),
            CipherMode.CFB => DecryptCfb(data),
            CipherMode.OFB => DecryptOfb(data),
            CipherMode.CTR => DecryptCtr(data),
            CipherMode.RandomDelta => DecryptRd(data),
            _ => throw new NotSupportedException()
        };

        result = Padding.Remove(decrypted, _padding);
    }

    public void Encrypt(string inputPath, string outputPath)
    {
        if (inputPath == null) throw new ArgumentNullException(nameof(inputPath));
        if (outputPath == null) throw new ArgumentNullException(nameof(outputPath));

        byte[] data = File.ReadAllBytes(inputPath);
        Encrypt(data, out byte[] result);
        File.WriteAllBytes(outputPath, result);
    }

    public void Decrypt(string inputPath, string outputPath)
    {
        if (inputPath == null) throw new ArgumentNullException(nameof(inputPath));
        if (outputPath == null) throw new ArgumentNullException(nameof(outputPath));

        byte[] data = File.ReadAllBytes(inputPath);
        Decrypt(data, out byte[] result);
        File.WriteAllBytes(outputPath, result);
    }

    private byte[] EncryptEcb(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        Parallel.For(0, blockCount, i =>
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);
            byte[] encrypted = _cipher.Encrypt(block);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private byte[] EncryptCbc(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        byte[] prev = _iv!;

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] xored = ByteUtils.Xor(block, prev);

            byte[] encrypted = _cipher.Encrypt(xored);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);

            prev = encrypted;
        }
        
        return result;
    }

    private byte[] EncryptPcbc(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        byte[] prevCipher = _iv!;
        byte[] prevMsg = new byte[blockSize];

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] tmp = ByteUtils.Xor(block, prevCipher);
            byte[] xored = ByteUtils.Xor(tmp, prevMsg);

            byte[] encrypted = _cipher.Encrypt(xored);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);

            prevCipher = encrypted;
            prevMsg = block;
        }
        return result;
    }

    private byte[] EncryptCfb(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        byte[] prevCipher = _iv!;

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] encrypted1 = _cipher.Encrypt(prevCipher);
            byte[] encrypted = ByteUtils.Xor(encrypted1, block);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);

            prevCipher = encrypted;
        }

        return result;
    }

    private byte[] EncryptOfb(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        byte[] prevCipher = _iv!;

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] encrypted1 = _cipher.Encrypt(prevCipher);
            byte[] encrypted = ByteUtils.Xor(encrypted1, block);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);

            prevCipher = encrypted1;
        }

        return result;
    }

    private byte[] EncryptCtr(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        byte[] result = new byte[padded.Length];

        byte[] iv = (byte[])_iv!.Clone();
        byte[] delta = new byte[blockSize];
        delta[blockSize - 1] = 1;

        Parallel.For(0, blockCount, i =>
        {
            byte[] counter = ComputeCounter(iv, delta, i);
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] keystream = _cipher.Encrypt(counter);
            byte[] encrypted = ByteUtils.Xor(block, keystream);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private byte[] EncryptRd(byte[] padded)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = padded.Length / blockSize;
        int halfSize = blockSize / 2;
        byte[] result = new byte[padded.Length];

        byte[] iv = (byte[])_iv!.Clone();
        byte[] delta = new byte[halfSize];
        Buffer.BlockCopy(_iv!, blockSize - halfSize, delta, 0, halfSize);

        Parallel.For(0, blockCount, i =>
        {
            byte[] counter = ComputeCounter(iv, delta, i);
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(padded, i * blockSize, block, 0, blockSize);

            byte[] keystream = _cipher.Encrypt(counter);
            byte[] encrypted = ByteUtils.Xor(block, keystream);
            Buffer.BlockCopy(encrypted, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private static byte[] ComputeCounter(byte[] iv, byte[] delta, int index)
    {
        byte[] counter = (byte[])iv.Clone();
        if (index == 0) return counter;

        int deltaLen = delta.Length;
        int offset = counter.Length - deltaLen;

        ulong carry = 0;
        for (int j = deltaLen - 1; j >= 0; j--)
        {
            ulong prod = (ulong)delta[j] * (ulong)index + carry + counter[offset + j];
            counter[offset + j] = (byte)prod;
            carry = prod >> 8;
        }

        for (int j = offset - 1; j >= 0 && carry > 0; j--)
        {
            ulong prod = carry + counter[j];
            counter[j] = (byte)prod;
            carry = prod >> 8;
        }

        return counter;
    }

    private byte[] DecryptEcb(byte[] data)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = data.Length / blockSize;
        byte[] result = new byte[data.Length];

        Parallel.For(0, blockCount, i =>
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(data, i * blockSize, block, 0, blockSize);

            byte[] decrypted = _cipher.Decrypt(block);
            Buffer.BlockCopy(decrypted, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private byte[] DecryptCbc(byte[] data)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = data.Length / blockSize;
        byte[] result = new byte[data.Length];

        byte[] iv = _iv!;

        Parallel.For(0, blockCount, i =>
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(data, i * blockSize, block, 0, blockSize);

            byte[] prev = new byte[blockSize];
            if (i == 0) Buffer.BlockCopy(iv, 0, prev, 0, blockSize);
            else Buffer.BlockCopy(data, (i - 1) * blockSize, prev, 0, blockSize);

            byte[] decrypted = _cipher.Decrypt(block);
            byte[] msg = ByteUtils.Xor(decrypted, prev);
            Buffer.BlockCopy(msg, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private byte[] DecryptPcbc(byte[] data)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = data.Length / blockSize;
        byte[] result = new byte[data.Length];

        byte[] prevCipher = _iv!;
        byte[] prevMsg = new byte[blockSize];

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(data, i * blockSize, block, 0, blockSize);

            byte[] decrypted = _cipher.Decrypt(block);

            byte[] tmp = ByteUtils.Xor(decrypted, prevMsg);
            byte[] msg = ByteUtils.Xor(tmp, prevCipher);

            Buffer.BlockCopy(msg, 0, result, i * blockSize, blockSize);

            prevCipher = block;
            prevMsg = msg;
        }

        return result;
    }

    private byte[] DecryptCfb(byte[] data)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = data.Length / blockSize;
        byte[] result = new byte[data.Length];

        byte[] iv = _iv!;

        Parallel.For(0, blockCount, i =>
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(data, i * blockSize, block, 0, blockSize);

            byte[] prev = new byte[blockSize];
            if (i == 0) Buffer.BlockCopy(iv, 0, prev, 0, blockSize);
            else Buffer.BlockCopy(data, (i - 1) * blockSize, prev, 0, blockSize);

            byte[] keystream = _cipher.Encrypt(prev);
            byte[] msg = ByteUtils.Xor(block, keystream);
            Buffer.BlockCopy(msg, 0, result, i * blockSize, blockSize);
        });

        return result;
    }

    private byte[] DecryptOfb(byte[] data)
    {
        int blockSize = _cipher.BlockSize;
        int blockCount = data.Length / blockSize;
        byte[] result = new byte[data.Length];

        byte[] prevCipher = _iv!;

        for (int i = 0; i < blockCount; i++)
        {
            byte[] block = new byte[blockSize];
            Buffer.BlockCopy(data, i * blockSize, block, 0, blockSize);

            byte[] encrypted = _cipher.Encrypt(prevCipher);
            byte[] msg = ByteUtils.Xor(block, encrypted);
            Buffer.BlockCopy(msg, 0, result, i * blockSize, blockSize);

            prevCipher = encrypted;
        }

        return result;
    }

    private byte[] DecryptCtr(byte[] data) => EncryptCtr(data);

    private byte[] DecryptRd(byte[] data) => EncryptRd(data);



    public Task<byte[]> EncryptAsync(byte[] data)
    {
        return Task.Run(() =>
        {
            Encrypt(data, out byte[] result);
            return result;
        });
    }

    public Task<byte[]> DecryptAsync(byte[] data)
    {
        return Task.Run(() =>
        {
            Decrypt(data, out byte[] result);
            return result;
        });
    }

    public Task EncryptAsync(string inputPath, string outputPath)
    {
        return Task.Run(() => Encrypt(inputPath, outputPath));
    }

    public Task DecryptAsync(string inputPath, string outputPath)
    {
        return Task.Run(() => Decrypt(inputPath, outputPath));
    }
}