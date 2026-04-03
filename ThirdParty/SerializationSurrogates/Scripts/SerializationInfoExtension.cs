using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Insthync.SerializationSurrogates
{
    public static class SerializationInfoExtension
    {
        public static void AddListValue<T>(this SerializationInfo info, string dataName, IList<T> data)
        {
            int count = data.Count;
            info.AddValue(dataName + "_Count", count);
            for (int i = 0; i < count; ++i)
            {
                info.AddValue(dataName + "_[" + i + "]", data[i]);
            }
        }

        public static IList<T> GetListValue<T>(this SerializationInfo info, string dataName)
        {
            List<T> result = new List<T>();
            int? count = null;
            try { count = info.GetInt32(dataName + "_Count"); }
            catch { }
            if (count.HasValue)
            {
                for (int i = 0; i < count.Value; ++i)
                {
                    result.Add((T)info.GetValue(dataName + "_[" + i + "]", typeof(T)));
                }
            }
            else
            {
                // Backward compatible
                try
                { result.AddRange((IList<T>)info.GetValue(dataName, typeof(IList<T>))); }
                catch { }
            }
            return result;
        }
    }
}
