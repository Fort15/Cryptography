using System;
using System.Numerics;
using Lab2.PrimalityTests;
using Lab2.NumberTheory;
using Lab2.RSA;
using Lab2.WienerAttack;

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

        DemonstratePrimalityTests();
        DemonstrateRSA();
        DemonstrateWienerAttack();
    }

    private static void DemonstratePrimalityTests()
    {
        IPrimalityTest fermat = new FermatTest();
        IPrimalityTest solovay = new SolovayStrassenTest();
        IPrimalityTest miller = new MillerRabinTest();

        BigInteger[] numbers = {
            -5, 0, 1, 2, 3, 4, 5, 7,
            9, 15, 17, 21, 25, 41, 97,
            341, 561, 1105, 1729, 7919,
            BigInteger.Parse("2305843009213693951")
        };

        double probability = 0.9999;

        Console.WriteLine("\n=== Primality tests ===");
        Console.WriteLine($"Probability: {probability}");

        Console.WriteLine($"{"Number",22} | {"Fermat",8} | " + $"{"Solovay",8} | {"Miller",8}");

        Console.WriteLine(new string('-', 57));

        foreach (BigInteger n in numbers)
        {
            bool f = fermat.IsPrime(n, probability);
            bool s = solovay.IsPrime(n, probability);
            bool m = miller.IsPrime(n, probability);

            Console.WriteLine($"{n,22} | {f,8} | {s,8} | {m,8}");
        }
    }

    private static void DemonstrateRSA()
    {
        Console.WriteLine("=== RSA ===");

        var rsa = new RSAService(RSAService.PrimalityTestType.MillerRabin, 0.99999, 128);

        BigInteger message = 12345;

        for (int i = 0; i < 2; i++)
        {
            var (n, e, d) = rsa.GenerateKeys();

            Console.WriteLine($"\nKey pair #{i + 1}");
            Console.WriteLine($"N = {n}");
            Console.WriteLine($"E = {e}");
            Console.WriteLine($"D = {d}");

            BigInteger encrypted = rsa.Encrypt(message);
            BigInteger decrypted = rsa.Decrypt(encrypted);

            Console.WriteLine($"Message:   {message}");
            Console.WriteLine($"Encrypted: {encrypted}");
            Console.WriteLine($"Decrypted: {decrypted}");

            Console.WriteLine($"Success: {message == decrypted}");
        }
    }

    private static void DemonstrateWienerAttack()
    {
        Console.WriteLine("\n=== Wiener attack ===");

        var generator = new VulnerableKeyGenerator(0.99999, 32);
        var attack = new WienerAttackService();

        for (int i = 0; i < 3; i++)
        {
            var (n, e, d, phi) = generator.GenerateKeys();

            Console.WriteLine($"\n=== Vulnerable RSA key #{i + 1} ===");

            Console.WriteLine($"N = {n}");
            Console.WriteLine($"E = {e}");
            Console.WriteLine($"D = {d}");
            Console.WriteLine($"Phi = {phi}");

            var (recoveredD, recoveredPhi, convergents) =
                attack.Attack(e, n);

            Console.WriteLine("\nConvergents:");

            foreach (var (numerator, denominator) in convergents)
            {
                Console.WriteLine($"{numerator}/{denominator}");
            }

            Console.WriteLine($"\nRecovered D = {recoveredD}");
            Console.WriteLine($"Recovered Phi = {recoveredPhi}");

            Console.WriteLine($"D match: {d == recoveredD}");
            Console.WriteLine($"Phi match: {phi == recoveredPhi}");

            Console.WriteLine($"Attack success: " + $"{d == recoveredD && phi == recoveredPhi}");
        }

        Console.WriteLine("\n=== Protected RSA key ===");

        var rsa = new RSAService(RSAService.PrimalityTestType.MillerRabin, 0.99999, 128);

        var (safeN, safeE, safeD) = rsa.GenerateKeys();

        Console.WriteLine($"N = {safeN}");
        Console.WriteLine($"E = {safeE}");
        Console.WriteLine($"D = {safeD}");

        var (foundD, foundPhi, safeConvergents) = attack.Attack(safeE, safeN);

        Console.WriteLine($"Convergents count: {safeConvergents.Count}");
        Console.WriteLine($"Attack success: {foundD != null}");
    }
}
