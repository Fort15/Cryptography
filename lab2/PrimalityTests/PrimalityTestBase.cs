using System;
using System.Numerics;
using System.Security.Cryptography;
using Lab2.NumberTheory;

namespace Lab2.PrimalityTests;

public abstract class PrimalityTestBase : IPrimalityTest
{
    protected NumberTheoryService NumTh { get; } = new NumberTheoryService();

    public bool IsPrime(BigInteger n, double probability)
    {
        if (!(probability >= 0.5 && probability < 1.0))
            throw new ArgumentException("Probability must be in [0.5, 1).", nameof(probability));

        if (n < 2) return false;
        if (n == 2 || n == 3) return true;
        if (n.IsEven) return false;

        BigInteger iterations = GetIterationCount(n, probability);

        for (BigInteger i = 0; i < iterations; i++)
        {
            BigInteger a = GenerateRandomNumber(n);
            if (!TestIteration(n, a)) return false;
        }

        return true;
    }

    protected abstract bool TestIteration(BigInteger n, BigInteger a);
    protected abstract BigInteger GetIterationCount(BigInteger n, double probability);

    private static BigInteger GenerateRandomNumber(BigInteger n)
    {
        BigInteger max = n - 4;
        byte[] bytes = max.ToByteArray(isUnsigned: true, isBigEndian: true);

        int mask = 1;
        while (mask < bytes[0])
        {
            mask = (mask << 1) | 1;
        }

        BigInteger result;

        do
        {
            RandomNumberGenerator.Fill(bytes);
            bytes[0] &= (byte)mask;
            result = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
        } while (result > max);

        return result + 2;
    }

    protected static int CalculateIterations(double probability, double errorProbability)
    {
        int iterations = 0;
        double error = 1.0;

        while (error > 1.0 - probability)
        {
            iterations++;
            error *= errorProbability;
        }

        return iterations;
    }
}