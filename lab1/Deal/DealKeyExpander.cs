using Cryptography.lab1.Des;
using Cryptography.lab1.Interfaces;

namespace Cryptography.lab1.Deal;

public class DealKeyExpander : IKeyExpander
{
    private static readonly byte[] ConstantKey = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

    private readonly DesCipher _desForKeyShedule;

    public DealKeyExpander()
    {
        _desForKeyShedule = new DesCipher();
        _desForKeyShedule.SetKey(ConstantKey);
    }
}