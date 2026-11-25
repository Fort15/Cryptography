using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab2.WienerAttack;

public static class ContinuedFraction
{
    public static List<BigInteger> GetCoefficients(BigInteger numerator, BigInteger denominator)
    {
        if (numerator < 0) throw new ArgumentOutOfRangeException(nameof(numerator));
        if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));

        List<BigInteger> coefficients = new List<BigInteger>();

        while (denominator != 0)
        {
            BigInteger quotient = numerator / denominator;
            coefficients.Add(quotient);

            BigInteger remainder = numerator % denominator;
            numerator = denominator;
            denominator = remainder;
        }

        return coefficients;
    }

    public static List<(BigInteger numerator, BigInteger denominator)> GetConvergents(List<BigInteger> coefficients)
    {
        List<(BigInteger numerator, BigInteger denominator)> convergents = new List<(BigInteger, BigInteger)>();

        BigInteger p0 = 0;
        BigInteger p1 = 1;
        BigInteger q0 = 1;
        BigInteger q1 = 0;

        foreach (var coefficient in coefficients)
        {
            BigInteger p2 = coefficient * p1 + p0;
            BigInteger q2 = coefficient * q1 + q0;

            convergents.Add((p2, q2));

            p0 = p1;
            p1 = p2;
            q0 = q1;
            q1 = q2;
        }

        return convergents;
    }
}