using System.Numerics;
using lab2.NumberTheory;

namespace Lab2.NumberTheory;

public sealed class NumberTheoryService
{
    public BigInteger PowMod(BigInteger a, BigInteger exponent, BigInteger modulus)
        => ModPow.Pow(a, exponent, modulus);

    public int Legendre(BigInteger a, BigInteger p)
        => LegendreSymbol.Compute(a, p);

    public int Jacobi(BigInteger a, BigInteger n)
        => JacobiSymbol.Compute(a, n);

    public BigInteger Gcd(BigInteger a, BigInteger b)
        => EuclideanAlgorithm.Compute(a, b);

    public (BigInteger Gcd, BigInteger X, BigInteger Y) ExtendedGcd(BigInteger a, BigInteger b)
        => ExtendedEuclideanAlgorithm.Compute(a, b);

    public BigInteger BinaryGcd(BigInteger a, BigInteger b)
        => BinaryEuclideanAlgorithm.Compute(a, b);

    public BigInteger EulerByDefinition(BigInteger n)
        => EulerValue.ByDefinition(n);

    public BigInteger EulerByFactorization(BigInteger n)
        => EulerValue.ByPrimeFactorization(n);

    public BigInteger EulerByFourier(BigInteger n)
        => EulerValue.ByDiscreteFourierTransform(n);

    public BigInteger Carmichael(BigInteger n)
        => CarmichaelValue.ByPrimeFactorization(n);
}
