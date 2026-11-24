using System;
using System.Numerics;

namespace Lab2.NumberTheory;


public static class ModPow
{
    public static BigInteger Pow(BigInteger a, BigInteger exponent, BigInteger mod)
    {
        if (mod <= 0) throw new ArgumentException("Modulus must be positive.", nameof(mod));
        if (exponent < 0) throw new ArgumentException("Exponent must be non-negative.", nameof(exponent));

        a %= mod;

        if (a < 0) a += mod;

        BigInteger result = BigInteger.One % mod;
        while (exponent > 0)
        {
            if (!exponent.IsEven) result = (result * a) % mod;
            a = (a * a) % mod;
            exponent /= 2;
        }
        return result;
    }
}
