using System;
using System.Numerics;

namespace Lab2.NumberTheory;

public static class LegendreSymbol
{
    public static int Compute(BigInteger a, BigInteger p)
    {
        if (p <= 2 || p.IsEven) throw new ArgumentException("p must be an odd prime.", nameof(p));
        a %= p;
        if (a < 0) a += p;

        if (a.IsZero) return 0;
        BigInteger exponent = (p - 1) / 2;
        BigInteger result = ModPow.Pow(a, exponent, p);

        if (result == BigInteger.One) return 1;
        if (result == p - 1) return -1;
        throw new InvalidOperationException("Unexpected result. Perhaps p is not prime.");
    }
}