using System;
using Lab2.NumberTheory;

internal class Program
{
    static void Main()
    {
        var service = new NumberTheoryService();

        Console.WriteLine("=== Modular exponentiation ===");
        Console.WriteLine($"3^13 mod 7 = {service.PowMod(3, 13, 7)}");

        Console.WriteLine("\n=== Legendre symbol ===");
        Console.WriteLine($"(3/7) = {service.Legendre(3, 7)}");

        Console.WriteLine("\n=== Jacobi symbol ===");
        Console.WriteLine($"(2/21) = {service.Jacobi(2, 21)}");

        Console.WriteLine("\n=== Euclidean algorithm ===");
        Console.WriteLine($"GCD(252, 105) = {service.Gcd(252, 105)}");

        Console.WriteLine("\n=== Extended Euclidean algorithm ===");
        var (gcd, x, y) = service.ExtendedGcd(252, 105);

        Console.WriteLine($"GCD = {gcd}, x = {x}, y = {y}");
        Console.WriteLine($"Bezout check: {252 * x + 105 * y}");

        Console.WriteLine("\n=== Binary Euclidean algorithm ===");
        Console.WriteLine($"GCD(252, 105) = {service.BinaryGcd(252, 105)}");

        Console.WriteLine("\n=== Euler totient function ===");
        Console.WriteLine($"Definition: {service.EulerByDefinition(12)}");
        Console.WriteLine($"Factorization: {service.EulerByFactorization(12)}");
        Console.WriteLine($"Fourier: {service.EulerByFourier(12)}");

        Console.WriteLine("\n=== Carmichael function ===");
        Console.WriteLine($"lambda(72) = {service.Carmichael(72)}");
    }
}
