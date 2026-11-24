using System;
using System.Numerics;
using Lab2.NumberTheory;

namespace lab2.NumberTheory;


public static class EulerValue
{
    public static BigInteger ByDefinition(BigInteger n)
    {
        if (n <= 0) throw new ArgumentException("n must be a positive integer.", nameof(n));
        BigInteger result = 0;

        for (BigInteger k = 1; k <= n; k++)
        {
            if (EuclideanAlgorithm.Compute(k, n) == 1) result++;
        }

        return result;
    }

    public static BigInteger ByPrimeFactorization(BigInteger n)
    {
        if (n <= 0) throw new ArgumentException("n must be a positive integer.", nameof(n));

        BigInteger result = n;
        BigInteger remaining = n;

        if (remaining % 2 == 0)
        {
            result -= result / 2;

            while (remaining % 2 == 0) remaining /= 2;
        }

        for (BigInteger p = 3; p * p <= remaining; p += 2)
        {
            if (remaining % p == 0)
            {
                result -= result / p;

                while (remaining % p == 0) remaining /= p;
            }
        }

        if (remaining > 1) result -= result / remaining;
        return result;
    }

    public static BigInteger ByDiscreteFourierTransform(BigInteger n)
    {
        if (n <= 0) throw new ArgumentException("n must be a positive integer.", nameof(n));
        double sum = 0;
        double nValue = (double)n;

        for (BigInteger k = 1; k <= n; k++)
        {
            BigInteger gcd = EuclideanAlgorithm.Compute(k, n);
            double angle = 2.0 * Math.PI * (double)k / nValue;
            sum += (double)gcd * Math.Cos(angle);
        }

        return new BigInteger(Math.Round(sum, MidpointRounding.AwayFromZero));
    }
}