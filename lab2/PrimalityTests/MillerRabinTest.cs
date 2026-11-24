using System.Numerics;
using Lab2.PrimalityTests;

namespace Lab2.PrimalityTests;

public sealed class MillerRabinTest : PrimalityTestBase
{
    protected override bool TestIteration(BigInteger n, BigInteger a)
    {
        if (NumTh.Gcd(a, n) != 1) return false;

        BigInteger s = n - 1;
        int d = 0;

        while (s.IsEven)
        {
            d++;
            s /= 2;
        }

        BigInteger x = NumTh.PowMod(a, s, n);
        if (x == 1 || x == n - 1) return true;
        
        for (int r = 1; r < d; ++r)
        {
            x = (x * x) % n;
            if (x == n - 1) return true;
            if (x == 1) return false;
        }

        return false;
    }

    protected override BigInteger GetIterationCount(BigInteger n, double probability)
    {
        return CalculateIterations(probability, 0.25);
    }
}