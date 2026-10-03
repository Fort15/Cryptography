namespace Cryptography.lab1.Interfaces;

public interface IRoundFunction
{
    int InputBlockSize { get; }
    int RoundKeySize { get; }
    int OutputBlockSize { get; }

    byte[] Apply(byte[] block, byte[] roundKey);
}
