namespace Insthync.AddressableAssetTools
{
    public interface IObjectRefToAddressableConversion
    {
        bool ShouldConvertToAddressable();
        void ConvertObjectRefToAddressable();
    }
}
