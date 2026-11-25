using System;
using System.Numerics;
using System.Security.Cryptography;
using Lab2.PrimalityTests;
using Lab2.NumberTheory;

namespace Lab2.RSA;

public sealed class RSAService
{
    public enum PrimalityTestType
    {
        Fermat,
        SolovayStrassen,
        MillerRabin
    }

    private readonly KeyGenerator _keyGenerator;
    private static readonly NumberTheoryService NumTh = new NumberTheoryService();
    private (BigInteger N, BigInteger E, BigInteger D)? _keys;

    public RSAService(PrimalityTestType testType, double probability, int bitLength)
    {
        _keyGenerator = new KeyGenerator(testType, probability, bitLength);
    }

    public (BigInteger N, BigInteger E, BigInteger D) GenerateKeys()
    {
        var keys = _keyGenerator.GenerateKeys();
        _keys = keys;
        return keys;
    }

    public BigInteger Encrypt(BigInteger message)
    {
        var keys = _keys ?? throw new InvalidOperationException("Keys have not been generated.");

        if (message < 0 || message >= keys.N)
            throw new ArgumentOutOfRangeException(nameof(message));

        return NumTh.PowMod(message, keys.E, keys.N);
    }
    public BigInteger Decrypt(BigInteger cipher)
    {
        var keys = _keys ?? throw new InvalidOperationException("Keys have not been generated.");

        if (cipher < 0 || cipher >= keys.N)
            throw new ArgumentOutOfRangeException(nameof(cipher));

        return NumTh.PowMod(cipher, keys.D, keys.N);
    }


    private sealed class KeyGenerator
    {
        private readonly IPrimalityTest _primalityTest;
        private readonly double _probability;
        private readonly int _bitLength;

        public KeyGenerator(PrimalityTestType testType, double probability, int bitLength)
        {
            if (!(probability >= 0.5 && probability < 1.0))
                throw new ArgumentException("Probability must be in [0.5, 1).", nameof(probability));

            if (bitLength < 128) throw new ArgumentException("Bit length must be at least 128.", nameof(bitLength));

            _probability = probability;
            _bitLength = bitLength;

            _primalityTest = testType switch
            {
                PrimalityTestType.Fermat => new FermatTest(),
                PrimalityTestType.SolovayStrassen => new SolovayStrassenTest(),
                PrimalityTestType.MillerRabin => new MillerRabinTest(),
                _ => throw new ArgumentOutOfRangeException(nameof(testType), "Invalid primality test type.")
            };
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

        public (BigInteger N, BigInteger E, BigInteger D) GenerateKeys()
        {
            BigInteger e = 65537;
            BigInteger minDistance = BigInteger.One << (_bitLength - 3);

            while (true)
            {
                BigInteger p = GeneratePrime();
                BigInteger q = GeneratePrime();

                if (BigInteger.Abs(p - q) < minDistance)
                    continue;

                BigInteger n = p * q;
                BigInteger phi = (p - 1) * (q - 1);

                if (e >= phi)
                    continue;

                var (gcd, x, _) = NumTh.ExtendedGcd(e, phi);

                if (gcd != 1) continue;

                BigInteger d = x % phi;
                if (d < 0) d += phi;

                if (d * d <= n) continue;

                return (n, e, d);
            }
        }
    }
}
