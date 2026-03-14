using System.Reflection;

namespace MultiplayerARPG
{
    public class FieldAttributeInfo<T> where T : Attribute
    {
        public FieldInfo Field { get; set; }
        public object Source { get; set; }
        public T AttributeInstance { get; set; }
    }
}