using System.Numerics;

namespace Lab2.PrimalityTests;

public interface IPrimalityTest
{
    bool IsPrime(BigInteger n, double probability);
}