namespace Cryptography.lab1.Contexts;


public enum CipherMode
{
    ECB,
    CBC,
    PCBC,
    CFB,
    OFB,
    CTR,
    RandomDelta
}

public enum PaddingMode
{
    Zeros,
    ANSIX923,
    PKCS7,
    ISO10126
}