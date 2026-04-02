namespace Insthync.AddressableAssetTools
{
    public class AddressableAssetConversionAttribute : System.Attribute
    {
        public string AddressableVarName { get; private set; }

        public AddressableAssetConversionAttribute(string addressableVarName)
        {
            AddressableVarName = addressableVarName;
        }
    }
}
