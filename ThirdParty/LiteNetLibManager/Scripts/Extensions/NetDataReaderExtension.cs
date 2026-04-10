using LiteNetLibManager.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtension
    {
        public static TType GetValue<TType>(this NetDataReader reader)
        {
            return (TType)GetValue(reader, typeof(TType));
        }

        public static object GetValue(this NetDataReader reader, Type type)
        {
            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            if (ReaderRegistry.TryGetReader(type, out Func<NetDataReader, object> readerFunc))
            {
                return readerFunc(reader);
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning($"No reader registered for type: {type.FullName}");
#endif
            if (typeof(INetSerializable).IsAssignableFrom(type))
            {
                object instance = Activator.CreateInstance(type);
                (instance as INetSerializable).Deserialize(reader);
                return instance;
            }

            throw new ArgumentException("NetDataReader cannot read type " + type.Name);
        }

        public static Color GetColor(this NetDataReader reader)
        {
            float r = reader.GetByte() * 0.01f;
            float g = reader.GetByte() * 0.01f;
            float b = reader.GetByte() * 0.01f;
            float a = reader.GetByte() * 0.01f;
            return new Color(r, g, b, a);
        }

        public static Quaternion GetQuaternion(this NetDataReader reader)
        {
            return new Quaternion(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2(reader.GetFloat(), reader.GetFloat());
        }

        public static Vector2Int GetVector2Int(this NetDataReader reader)
        {
            return new Vector2Int(reader.GetInt(), reader.GetInt());
        }

        public static Vector3 GetVector3(this NetDataReader reader)
        {
            return new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        /// <summary>
        ///Read a quantized Vector3 from the reader, the vector is quantized into integers based on the cell size and compression mode, which determines how many bits are used for each component.
        public static Vector3 GetQuantizedVector3(this NetDataReader reader, ushort cellSize, out int compressionMode)
        {
            //Read Mode from the first byte, the mode is determined by the top 2 bits of the first byte, and the remaining 6 bits are used for quantized data. The mode determines how many bits are used for each component of the vector, which can be 3, 4, 5, or 6 bits for x, y, and z respectively.
            byte first = reader.GetByte();
            compressionMode = ((first >> 6) & 0b11) + 3;

            int bx, by, bz;

            switch (compressionMode)
            {
                case 3: bx = 10; by = 4; bz = 10; break;
                case 4: bx = 11; by = 10; bz = 11; break;
                case 5: bx = 14; by = 12; bz = 14; break;
                case 6: bx = 16; by = 16; bz = 16; break;
                default: throw new Exception("Invalid mode");
            }

            int totalBits = bx + by + bz;
            int byteCount = (totalBits + 7) / 8;

            ulong data = (ulong)(first & 0x3F); // first 6 bits
            int shift = 6;

            for (int i = 1; i < byteCount; i++)
            {
                byte b = reader.GetByte();
                data |= ((ulong)b << shift);
                shift += 8;
            }


            //Extract quantized values for x, y, and z from the combined data using bitwise operations. The values are extracted in the order of x, z, and y, based on the number of bits allocated for each component.
            int s = 0;

            int qx = (int)((data >> s) & ((1UL << bx) - 1)); s += bx;
            int qz = (int)((data >> s) & ((1UL << bz) - 1)); s += bz;
            int qy = (int)((data >> s) & ((1UL << by) - 1));

            float x = Dequantize(qx, cellSize, bx);
            float y = Dequantize(qy, cellSize, by);
            float z = Dequantize(qz, cellSize, bz);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Dequantize an integer value to a float based on the cell size and the number of bits used for quantization. The value is first normalized to the range [0, 1] by dividing it by the maximum integer value that can be represented with the given number of bits, and then scaled by the cell size to get the final float value.
        static float Dequantize(int value, ushort cellSize, int bits)
        {
            float maxInt = (1 << bits) - 1;
            return ((float)value / maxInt) * cellSize;
        }

        public static Vector3Int GetVector3Int(this NetDataReader reader)
        {
            return new Vector3Int(reader.GetInt(), reader.GetInt(), reader.GetInt());
        }

        public static Vector4 GetVector4(this NetDataReader reader)
        {
            return new Vector4(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
        }

        public static TValue[] GetArrayExtension<TValue>(this NetDataReader reader)
        {
            int count = reader.GetInt();
            TValue[] result = new TValue[count];
            for (int i = 0; i < count; ++i)
            {
                result[i] = reader.GetValue<TValue>();
            }
            return result;
        }

        public static object GetArrayObject(this NetDataReader reader, Type type)
        {
            int count = reader.GetInt();
            Array array = Array.CreateInstance(type, count);
            for (int i = 0; i < count; ++i)
            {
                array.SetValue(reader.GetValue(type), i);
            }
            return array;
        }

        public static List<TValue> GetList<TValue>(this NetDataReader reader)
        {
            int count = reader.GetInt();
            List<TValue> result = new List<TValue>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(reader.GetValue<TValue>());
            }
            return result;
        }

        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(this NetDataReader reader)
        {
            int count = reader.GetInt();
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>();
            for (int i = 0; i < count; ++i)
            {
                result.Add(reader.GetValue<TKey>(), reader.GetValue<TValue>());
            }
            return result;
        }

        #region Packed Signed Int (Ref: https://developers.google.com/protocol-buffers/docs/encoding#signed-integers)
        public static short GetPackedShort(this NetDataReader reader)
        {
            return (short)GetPackedInt(reader);
        }

        public static int GetPackedInt(this NetDataReader reader)
        {
            uint value = GetPackedUInt(reader);
            return (int)((value >> 1) ^ (-(int)(value & 1)));
        }

        public static long GetPackedLong(this NetDataReader reader)
        {
            return ((long)GetPackedInt(reader)) << 32 | ((uint)GetPackedInt(reader));
        }
        #endregion

        #region Packed Unsigned Int (Ref: https://sqlite.org/src4/doc/trunk/www/varint.wiki)
        public static ushort GetPackedUShort(this NetDataReader reader)
        {
            return (ushort)GetPackedULong(reader);
        }

        public static uint GetPackedUInt(this NetDataReader reader)
        {
            return (uint)GetPackedULong(reader);
        }

        public static ulong GetPackedULong(this NetDataReader reader)
        {
            byte a0 = reader.GetByte();
            if (a0 < 241)
            {
                return a0;
            }

            byte a1 = reader.GetByte();
            if (a0 >= 241 && a0 <= 248)
            {
                return 240 + 256 * (a0 - ((ulong)241)) + a1;
            }

            byte a2 = reader.GetByte();
            if (a0 == 249)
            {
                return 2288 + (((ulong)256) * a1) + a2;
            }

            byte a3 = reader.GetByte();
            if (a0 == 250)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16);
            }

            byte a4 = reader.GetByte();
            if (a0 == 251)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24);
            }

            byte a5 = reader.GetByte();
            if (a0 == 252)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32);
            }

            byte a6 = reader.GetByte();
            if (a0 == 253)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40);
            }

            byte a7 = reader.GetByte();
            if (a0 == 254)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40) + (((ulong)a7) << 48);
            }

            byte a8 = reader.GetByte();
            if (a0 == 255)
            {
                return a1 + (((ulong)a2) << 8) + (((ulong)a3) << 16) + (((ulong)a4) << 24) + (((ulong)a5) << 32) + (((ulong)a6) << 40) + (((ulong)a7) << 48) + (((ulong)a8) << 56);
            }
            throw new System.IndexOutOfRangeException("ReadPackedULong() failure: " + a0);
        }
        #endregion
    }
}
