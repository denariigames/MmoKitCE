using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiplayerARPG
{
    public static class ReflectionUtils
    {
        public static List<FieldSourceInfo> FindFieldsOfType<T>(this object instance)
        {
            // To store all matching fields and their sources
            var matchingFields = new List<FieldSourceInfo>();

            // Call the recursive helper function
            FindFieldsOfTypeRecursive(instance, typeof(T), matchingFields);

            return matchingFields;
        }

        private static void FindFieldsOfTypeRecursive(object instance, System.Type targetType, List<FieldSourceInfo> matchingFields, HashSet<object> visited = null)
        {
            if (instance == null || targetType == null)
                return;

            // To avoid cyclic references
            visited ??= new HashSet<object>();
            if (visited.Contains(instance))
                return;

            visited.Add(instance);

            var fields = GetAllFields(instance.GetType());

            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(instance);

                // Check if field type matches the target type
                if (field.FieldType.IsSubclassOf(targetType))
                {
                    matchingFields.Add(new FieldSourceInfo { Field = field, Source = instance });
                }
                // Check if the field is a collection (list or array)
                else if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string))
                {
                    if (fieldValue is IList enumerable && enumerable != null)
                    {
                        foreach (var element in enumerable)
                        {
                            if (element != null)
                            {
                                // If the element type matches the target type
                                if (element.GetType() == targetType)
                                {
                                    matchingFields.Add(new FieldSourceInfo { Field = field, Source = instance });
                                    break; // Avoid adding the same field multiple times
                                }
                                // If the element is a class, recursively search its fields
                                else if (element.GetType().IsClass && element.GetType() != typeof(string))
                                {
                                    FindFieldsOfTypeRecursive(element, targetType, matchingFields, visited);
                                }
                            }
                        }
                    }
                }
                // Recursively search fields within fields of class type (excluding strings to avoid going into characters)
                else if (field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    FindFieldsOfTypeRecursive(fieldValue, targetType, matchingFields, visited);
                }
            }
        }

        public static List<FieldAttributeInfo<TAttribute>> FindFieldsWithAttribute<TAttribute>(this object instance)
            where TAttribute : Attribute
        {
            var matchingFields = new List<FieldAttributeInfo<TAttribute>>();
            FindFieldsWithAttributeRecursive(instance, typeof(TAttribute), matchingFields);
            return matchingFields;
        }

        private static void FindFieldsWithAttributeRecursive<TAttribute>(
            object instance,
            System.Type attributeType,
            List<FieldAttributeInfo<TAttribute>> matchingFields,
            HashSet<object> visited = null)
            where TAttribute : Attribute
        {
            if (instance == null || attributeType == null)
                return;

            visited ??= new HashSet<object>();
            if (visited.Contains(instance))
                return;

            visited.Add(instance);

            // Get all fields from the current type and base types
            var fields = GetAllFields(instance.GetType());

            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(instance);

                // Check if the field has the specified attribute
                var attribute = field.GetCustomAttributes(attributeType, false).FirstOrDefault() as TAttribute;
                if (attribute != null)
                {
                    matchingFields.Add(new FieldAttributeInfo<TAttribute> { Field = field, Source = instance, AttributeInstance = attribute });
                }
                // Check if the field is a collection (list or array) but not a string
                else if (typeof(IList).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string))
                {
                    if (fieldValue is IList enumerable && enumerable != null)
                    {
                        foreach (var element in enumerable)
                        {
                            if (element != null && element.GetType().IsClass && element.GetType() != typeof(string))
                            {
                                // Recursively check elements within collections
                                FindFieldsWithAttributeRecursive(element, attributeType, matchingFields, visited);
                            }
                        }
                    }
                }
                // Recursively search fields within class type fields (excluding strings)
                else if (field.FieldType.IsClass && field.FieldType != typeof(string))
                {
                    FindFieldsWithAttributeRecursive(fieldValue, attributeType, matchingFields, visited);
                }
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(this System.Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            // Traverse the inheritance hierarchy to get fields from base types
            var fields = new List<FieldInfo>();
            while (type != null && type != typeof(object))
            {
                fields.AddRange(type.GetFields(flags));
                type = type.BaseType;
            }
            return fields;
        }

        public static bool HasAttribute<TAttributeType>(this FieldInfo field, bool inherit = false)
            where TAttributeType : System.Attribute
        {
            return field.GetCustomAttributes(typeof(TAttributeType), inherit).Length > 0;
        }

        public static bool HasAttribute<TAttributeType>(this System.Type type, bool inherit = false)
            where TAttributeType : System.Attribute
        {
            return type.GetCustomAttributes(typeof(TAttributeType), inherit).Length > 0;
        }

        public static bool HasInterface<TInterfaceType>(this System.Type type)
        {
            foreach (System.Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType == typeof(TInterfaceType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsListOrArray(this System.Type type, out System.Type itemType)
        {
            if (type.IsArray)
            {
                itemType = type.GetElementType();
                return true;
            }
            foreach (System.Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    itemType = type.GetGenericArguments()[0];
                    return true;
                }
            }
            itemType = null;
            return false;
        }
    }
}
