using System;
using System.Numerics;
using System.Security.Cryptography;
using Lab2.NumberTheory;
using Lab2.PrimalityTests;

namespace Lab2.WienerAttack;

public sealed class VulnerableKeyGenerator
{
    private readonly NumberTheoryService NumTh = new NumberTheoryService();

    private readonly IPrimalityTest _primalityTest = new MillerRabinTest();

    private readonly double _probability;
    private readonly int _bitLength;

    public VulnerableKeyGenerator(double probability, int bitLength)
    {
        if (!(probability >= 0.5 && probability < 1.0))
            throw new ArgumentOutOfRangeException(nameof(probability));

        if (bitLength < 16)
            throw new ArgumentOutOfRangeException(nameof(bitLength));

        _probability = probability;
        _bitLength = bitLength;
    }

    private BigInteger GenerateRandomNumber()
    {
        int byteCount = (_bitLength + 7) / 8;
        int extraBits = byteCount * 8 - _bitLength;

        byte[] bytes = new byte[byteCount];

        RandomNumberGenerator.Fill(bytes);

        bytes[0] &= (byte)(255 >> extraBits);
        bytes[0] |= (byte)(1 << (7 - extraBits));
        bytes[bytes.Length - 1] |= 1;

        return new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
    }

    private BigInteger GeneratePrime()
    {
        BigInteger candidate;

        do
        {
            candidate = GenerateRandomNumber();
        } while (!_primalityTest.IsPrime(candidate, _probability));

        return candidate;
    }

    public (BigInteger N, BigInteger E, BigInteger D, BigInteger Phi) GenerateKeys()
    {
        while (true)
        {
            BigInteger p = GeneratePrime();
            BigInteger q = GeneratePrime();

            if (p == q)
                continue;

            if (p < q)
                (p, q) = (q, p);

            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);

            for (BigInteger d = 3; 81 * d * d * d * d < n; d += 2)
            {
                var (gcd, x, _) = NumTh.ExtendedGcd(d, phi);

                if (gcd != 1)
                    continue;

                BigInteger e = x % phi;

                if (e < 0) e += phi;

                return (n, e, d, phi);
            }
        }
    }
}
