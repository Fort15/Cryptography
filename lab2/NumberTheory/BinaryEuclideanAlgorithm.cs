using System;
using System.Numerics;

namespace Lab2.NumberTheory;

public static class BinaryEuclideanAlgorithm
{
    public static BigInteger Compute(BigInteger a, BigInteger b)
    {
        a = BigInteger.Abs(a);
        b = BigInteger.Abs(b);

        if (a.IsZero) return b;
        if (b.IsZero) return a;

        int k = 0;
        while (a.IsEven && b.IsEven)
        {
            a /= 2;
            b /= 2;
            k++;
        }

        while (a.IsEven) a /= 2;

        while (b != 0)
        {
            while (b.IsEven) b /= 2;

            if (a > b)
            {
                var tmp = a;
                a = b;
                b = tmp;
            }

            b -= a;
        }

        return a << k;
    }
}
