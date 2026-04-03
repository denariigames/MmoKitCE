using System.Runtime.Serialization;
using UnityEngine;

namespace Insthync.SerializationSurrogates
{
    sealed class Vector4SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj,
                                  SerializationInfo info, StreamingContext context)
        {
            Vector4 v4 = (Vector4)obj;
            info.AddValue("x", v4.x);
            info.AddValue("y", v4.y);
            info.AddValue("z", v4.z);
            info.AddValue("w", v4.w);
        }

        public object SetObjectData(object obj,
                                           SerializationInfo info, StreamingContext context,
                                           ISurrogateSelector selector)
        {
            Vector4 v4 = (Vector4)obj;
            v4.x = (float)info.GetValue("x", typeof(float));
            v4.y = (float)info.GetValue("y", typeof(float));
            v4.z = (float)info.GetValue("z", typeof(float));
            v4.w = (float)info.GetValue("w", typeof(float));
            obj = v4;
            return obj;
        }
    }
}
