using System;
using System.Numerics;

namespace Lab2.NumberTheory;

public static class JacobiSymbol
{
    public static int Compute(BigInteger a, BigInteger n)
    {
        if (n <= 0 || n.IsEven) throw new ArgumentException("n must be a positive odd integer.", nameof(n));

        a %= n;
        if (a < 0) a += n;

        int sign = 1;

        while (a != 0)
        {
            while (a.IsEven)
            {
                a /= 2;
                BigInteger nMod8 = n % 8;
                if (nMod8 == 3 || nMod8 == 5) sign = -sign;
            }

            BigInteger temp = a;
            a = n;
            n = temp;

            if (a % 4 == 3 && n % 4 == 3) sign = -sign;
            a %= n;
        }

        return n == 1 ? sign : 0;
    }
}
