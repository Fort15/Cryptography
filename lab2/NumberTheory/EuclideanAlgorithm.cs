using System;
using System.Numerics;

namespace Lab2.NumberTheory;

public static class EuclideanAlgorithm
{
    public static BigInteger Compute(BigInteger a, BigInteger b)
    {
        a = BigInteger.Abs(a);
        b = BigInteger.Abs(b);

        while (b != 0)
        {
            BigInteger remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    }
}
