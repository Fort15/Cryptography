using System;
using System.Numerics;
using Lab2.NumberTheory;

namespace lab2.NumberTheory;

public static class CarmichaelValue
{
    public static BigInteger ByPrimeFactorization(BigInteger n)
    {
        if (n <= 0) throw new ArgumentException("n must be a positive integer.", nameof(n));
        if (n == 1) return BigInteger.One;

        BigInteger remaining = n;
        BigInteger result = 1;

        int exponent = 0;
        while (remaining % 2 == 0)
        {
            remaining /= 2;
            exponent++;
        }

        if (exponent > 0)
        {
            BigInteger lambda;
            if (exponent == 1) lambda = 1;
            else if (exponent == 2) lambda = 2;
            else lambda = BigInteger.One << (exponent - 2);

            result = Lcm(result, lambda);
        }

        for (BigInteger p = 3; p * p <= remaining; p += 2)
        {
            if (remaining % p != 0) continue;

            exponent = 0;
            while (remaining % p == 0)
            {
                remaining /= p;
                exponent++;
            }
            
            BigInteger lambda = BigInteger.Pow(p, exponent - 1) * (p - 1);
            result = Lcm(result, lambda);
        }

        if (remaining > 1)
        {
            BigInteger lambda = remaining - 1;
            result = Lcm(result, lambda);
        }

        return result;
    }

    private static BigInteger Lcm(BigInteger a, BigInteger b)
    {
        return a / EuclideanAlgorithm.Compute(a, b) * b;
    }
}
