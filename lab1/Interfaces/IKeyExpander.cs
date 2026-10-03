namespace Cryptography.lab1.Interfaces;

public interface IKeyExpander
{
    int KeySize { get; }
    int Rounds { get; }
    byte[][] ExpandKey(byte[] key);
}