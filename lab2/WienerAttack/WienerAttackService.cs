using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab2.WienerAttack;

public sealed class WienerAttackService
{
    private static BigInteger IntegerSqrt(BigInteger n)
    {
        if (n < 0)
            throw new ArgumentException(nameof(n));

        if (n <= 1)
            return n;

        BigInteger left = 1;
        BigInteger right = n;

        while (left <= right)
        {
            BigInteger mid = (left + right) / 2;
            BigInteger square = mid * mid;
            if (square == n) return mid;
            if (square < n) left = mid + 1;
            else right = mid - 1;
        }

        return right;
    }

    public (BigInteger? d, BigInteger? phi, 
        List<(BigInteger numerator, BigInteger denominator)> convergents) Attack(BigInteger e, BigInteger n)
    {
        if (n <= 1) throw new ArgumentOutOfRangeException(nameof(n));
        if (e <= 1 || e >= n) throw new ArgumentOutOfRangeException(nameof(e));

        var coefficients = ContinuedFraction.GetCoefficients(e, n);
        var convergents = ContinuedFraction.GetConvergents(coefficients);

        foreach (var (k, d) in convergents)
        {
            if (k == 0) continue;

            BigInteger value = e * d - 1;
            if (value % k != 0) continue;

            BigInteger phi = value / k;

            if (phi <= 0 || phi >= n) continue;

            BigInteger sum = n - phi + 1;
            BigInteger D = sum * sum - 4 * n;
            if (D < 0) continue;

            BigInteger root = IntegerSqrt(D);
            if (root * root != D) continue;

            if ((sum + root) % 2 != 0) continue;

            BigInteger p = (sum + root) / 2;
            BigInteger q = (sum - root) / 2;

            if (p <= 1 || q <= 1 || p == q || p * q != n) continue;

            return (d, phi, convergents);
        }

        return (null, null, convergents);
    }
}