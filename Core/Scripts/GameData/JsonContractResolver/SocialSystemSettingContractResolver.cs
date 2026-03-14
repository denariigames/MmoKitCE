using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace MultiplayerARPG
{
    public class SocialSystemSettingContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = instance =>
            {
                if (member.DeclaringType == typeof(SocialSystemSetting) && !(member is PropertyInfo propertyInfo))
                    return false;
                return true;
            };
            return property;
        }
    }
}
