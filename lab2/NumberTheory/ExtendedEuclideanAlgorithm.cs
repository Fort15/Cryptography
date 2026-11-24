using System;
using System.Numerics;

namespace Lab2.NumberTheory;

public static class ExtendedEuclideanAlgorithm
{
    public static (BigInteger Gcd, BigInteger x, BigInteger y) Compute(BigInteger a, BigInteger b)
    {
        int signA = a.Sign < 0 ? -1 : 1;
        int signB = b.Sign < 0 ? -1 : 1;

        BigInteger r0 = BigInteger.Abs(a);
        BigInteger r1 = BigInteger.Abs(b);

        BigInteger x0 = 1;
        BigInteger y0 = 0;

        BigInteger x1 = 0;
        BigInteger y1 = 1;

        BigInteger r2, x2, y2;

        while (r1 != 0)
        {
            BigInteger q = r0 / r1;
            r2 = r0 - q * r1;
            x2 = x0 - q * x1;
            y2 = y0 - q * y1;

            r0 = r1; x0 = x1; y0 = y1;
            r1 = r2; x1 = x2; y1 = y2;
        } 

        return (r0, x0 * signA, y0 * signB);
    }
}