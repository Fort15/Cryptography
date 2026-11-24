using System.Numerics;

namespace Lab2.PrimalityTests;

public sealed class SolovayStrassenTest : PrimalityTestBase
{
    protected override bool TestIteration(BigInteger n, BigInteger a)
    {
        if (NumTh.Gcd(a, n) != 1) return false;

        BigInteger left = NumTh.PowMod(a, (n - 1) / 2, n);
        BigInteger right = NumTh.Jacobi(a, n);

        if (right < 0) right += n;
        return left == right;
    }

    protected override BigInteger GetIterationCount(BigInteger n, double probability)
    {
        return CalculateIterations(probability, 0.5);
    }
}