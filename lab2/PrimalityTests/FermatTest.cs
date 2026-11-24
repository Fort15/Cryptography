using System.Numerics;

namespace Lab2.PrimalityTests;

public sealed class FermatTest : PrimalityTestBase
{
    protected override bool TestIteration(BigInteger n, BigInteger a)
    {
        if (NumTh.Gcd(a, n) != 1) return false;
        return NumTh.PowMod(a, n - 1, n) == 1;
    }

    protected override BigInteger GetIterationCount(BigInteger n, double probability)
    {
        return CalculateIterations(probability, 0.5);
    }
}