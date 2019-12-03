using System;
using System.Collections.Generic;
using Engine.OpenCL;
using Engine.OpenCL.DotNetCore.DataTypes;

namespace Engine.OpenFL
{
    /// <summary>
    /// Used to magically convert VectorN into VectorN of different type.
    /// 
    /// </summary>
    public static class ClTypeConverter
    {
        //private delegate decimal ConvertRange(decimal value, decimal max);
        /// <summary>
        /// Dictionary containing the ToConverter
        /// From Array of objects(need to be implicitly converted into to the specifed base type)
        /// </summary>
        private static Dictionary<OpenCL.TypeEnums.DataTypes, ConvertToN> _toConverter =
            new Dictionary<OpenCL.TypeEnums.DataTypes, ConvertToN>
            {
                {OpenCL.TypeEnums.DataTypes.Uchar2, create_byte2},
                {OpenCL.TypeEnums.DataTypes.Uchar3, create_byte3},
                {OpenCL.TypeEnums.DataTypes.Uchar4, create_byte4},
                {OpenCL.TypeEnums.DataTypes.Uchar8, create_byte8},
                {OpenCL.TypeEnums.DataTypes.Uchar16, create_byte16},
                {OpenCL.TypeEnums.DataTypes.Char2, create_sbyte2},
                {OpenCL.TypeEnums.DataTypes.Char3, create_sbyte3},
                {OpenCL.TypeEnums.DataTypes.Char4, create_sbyte4},
                {OpenCL.TypeEnums.DataTypes.Char8, create_sbyte8},
                {OpenCL.TypeEnums.DataTypes.Char16, create_sbyte16},

                {OpenCL.TypeEnums.DataTypes.Ulong2, create_ulong2},
                {OpenCL.TypeEnums.DataTypes.Ulong3, create_ulong3},
                {OpenCL.TypeEnums.DataTypes.Ulong4, create_ulong4},
                {OpenCL.TypeEnums.DataTypes.Ulong8, create_ulong8},
                {OpenCL.TypeEnums.DataTypes.Ulong16, create_ulong16},
                {OpenCL.TypeEnums.DataTypes.Long2, create_long2},
                {OpenCL.TypeEnums.DataTypes.Long3, create_long3},
                {OpenCL.TypeEnums.DataTypes.Long4, create_long4},
                {OpenCL.TypeEnums.DataTypes.Long8, create_long8},
                {OpenCL.TypeEnums.DataTypes.Long16, create_long16},

                {OpenCL.TypeEnums.DataTypes.Uint2, create_uint2},
                {OpenCL.TypeEnums.DataTypes.Uint3, create_uint3},
                {OpenCL.TypeEnums.DataTypes.Uint4, create_uint4},
                {OpenCL.TypeEnums.DataTypes.Uint8, create_uint8},
                {OpenCL.TypeEnums.DataTypes.Uint16, create_uint16},
                {OpenCL.TypeEnums.DataTypes.Int2, create_int2},
                {OpenCL.TypeEnums.DataTypes.Int3, create_int3},
                {OpenCL.TypeEnums.DataTypes.Int4, create_int4},
                {OpenCL.TypeEnums.DataTypes.Int8, create_int8},
                {OpenCL.TypeEnums.DataTypes.Int16, create_int16},

                {OpenCL.TypeEnums.DataTypes.Ushort2, create_ushort2},
                {OpenCL.TypeEnums.DataTypes.Ushort3, create_ushort3},
                {OpenCL.TypeEnums.DataTypes.Ushort4, create_short4},
                {OpenCL.TypeEnums.DataTypes.Ushort8, create_ushort8},
                {OpenCL.TypeEnums.DataTypes.Ushort16, create_ushort16},
                {OpenCL.TypeEnums.DataTypes.Short2, create_short2},
                {OpenCL.TypeEnums.DataTypes.Short3, create_short3},
                {OpenCL.TypeEnums.DataTypes.Short4, create_short4},
                {OpenCL.TypeEnums.DataTypes.Short8, create_short8},
                {OpenCL.TypeEnums.DataTypes.Short16, create_short16},

                {OpenCL.TypeEnums.DataTypes.Float2, create_float2},
                {OpenCL.TypeEnums.DataTypes.Float3, create_float3},
                {OpenCL.TypeEnums.DataTypes.Float4, create_float4},
                {OpenCL.TypeEnums.DataTypes.Float8, create_float8},
                {OpenCL.TypeEnums.DataTypes.Float16, create_float16}
            };

        /// <summary>
        /// A dictionary containing the Base types for the different CL typee
        /// </summary>
        private static Dictionary<OpenCL.TypeEnums.DataTypes, Type> _baseTypes =
            new Dictionary<OpenCL.TypeEnums.DataTypes, Type>
            {
                {OpenCL.TypeEnums.DataTypes.Uchar2, typeof(byte)},
                {OpenCL.TypeEnums.DataTypes.Uchar3, typeof(byte)},
                {OpenCL.TypeEnums.DataTypes.Uchar4, typeof(byte)},
                {OpenCL.TypeEnums.DataTypes.Uchar8, typeof(byte)},
                {OpenCL.TypeEnums.DataTypes.Uchar16, typeof(byte)},
                {OpenCL.TypeEnums.DataTypes.Char2, typeof(sbyte)},
                {OpenCL.TypeEnums.DataTypes.Char3, typeof(sbyte)},
                {OpenCL.TypeEnums.DataTypes.Char4, typeof(sbyte)},
                {OpenCL.TypeEnums.DataTypes.Char8, typeof(sbyte)},
                {OpenCL.TypeEnums.DataTypes.Char16, typeof(sbyte)},

                {OpenCL.TypeEnums.DataTypes.Ulong2, typeof(ulong)},
                {OpenCL.TypeEnums.DataTypes.Ulong3, typeof(ulong)},
                {OpenCL.TypeEnums.DataTypes.Ulong4, typeof(ulong)},
                {OpenCL.TypeEnums.DataTypes.Ulong8, typeof(ulong)},
                {OpenCL.TypeEnums.DataTypes.Ulong16, typeof(ulong)},
                {OpenCL.TypeEnums.DataTypes.Long2, typeof(long)},
                {OpenCL.TypeEnums.DataTypes.Long3, typeof(long)},
                {OpenCL.TypeEnums.DataTypes.Long4, typeof(long)},
                {OpenCL.TypeEnums.DataTypes.Long8, typeof(long)},
                {OpenCL.TypeEnums.DataTypes.Long16, typeof(long)},

                {OpenCL.TypeEnums.DataTypes.Uint2, typeof(uint)},
                {OpenCL.TypeEnums.DataTypes.Uint3, typeof(uint)},
                {OpenCL.TypeEnums.DataTypes.Uint4, typeof(uint)},
                {OpenCL.TypeEnums.DataTypes.Uint8, typeof(uint)},
                {OpenCL.TypeEnums.DataTypes.Uint16, typeof(uint)},
                {OpenCL.TypeEnums.DataTypes.Int2, typeof(int)},
                {OpenCL.TypeEnums.DataTypes.Int3, typeof(int)},
                {OpenCL.TypeEnums.DataTypes.Int4, typeof(int)},
                {OpenCL.TypeEnums.DataTypes.Int8, typeof(int)},
                {OpenCL.TypeEnums.DataTypes.Int16, typeof(int)},

                {OpenCL.TypeEnums.DataTypes.Ushort2, typeof(ushort)},
                {OpenCL.TypeEnums.DataTypes.Ushort3, typeof(ushort)},
                {OpenCL.TypeEnums.DataTypes.Ushort4, typeof(short)},
                {OpenCL.TypeEnums.DataTypes.Ushort8, typeof(ushort)},
                {OpenCL.TypeEnums.DataTypes.Ushort16, typeof(ushort)},
                {OpenCL.TypeEnums.DataTypes.Short2, typeof(short)},
                {OpenCL.TypeEnums.DataTypes.Short3, typeof(short)},
                {OpenCL.TypeEnums.DataTypes.Short4, typeof(short)},
                {OpenCL.TypeEnums.DataTypes.Short8, typeof(short)},
                {OpenCL.TypeEnums.DataTypes.Short16, typeof(short)},

                {OpenCL.TypeEnums.DataTypes.Float2, typeof(float)},
                {OpenCL.TypeEnums.DataTypes.Float3, typeof(float)},
                {OpenCL.TypeEnums.DataTypes.Float4, typeof(float)},
                {OpenCL.TypeEnums.DataTypes.Float8, typeof(float)},
                {OpenCL.TypeEnums.DataTypes.Float16, typeof(float)}
            };

        /// <summary>
        /// Dictionary containing the FromConverter
        /// From the CL Type to an Array of objects
        /// </summary>
        private static Dictionary<OpenCL.TypeEnums.DataTypes, ConvertFromN> _fromConverter =
            new Dictionary<OpenCL.TypeEnums.DataTypes, ConvertFromN>
            {
                {OpenCL.TypeEnums.DataTypes.Uchar2, from_byte2},
                {OpenCL.TypeEnums.DataTypes.Uchar3, from_byte3},
                {OpenCL.TypeEnums.DataTypes.Uchar4, from_byte4},
                {OpenCL.TypeEnums.DataTypes.Uchar8, from_byte8},
                {OpenCL.TypeEnums.DataTypes.Uchar16, from_byte16},
                {OpenCL.TypeEnums.DataTypes.Char2, from_sbyte2},
                {OpenCL.TypeEnums.DataTypes.Char3, from_sbyte3},
                {OpenCL.TypeEnums.DataTypes.Char4, from_sbyte4},
                {OpenCL.TypeEnums.DataTypes.Char8, from_sbyte8},
                {OpenCL.TypeEnums.DataTypes.Char16, from_sbyte16},

                {OpenCL.TypeEnums.DataTypes.Ulong2, from_ulong2},
                {OpenCL.TypeEnums.DataTypes.Ulong3, from_ulong3},
                {OpenCL.TypeEnums.DataTypes.Ulong4, from_ulong4},
                {OpenCL.TypeEnums.DataTypes.Ulong8, from_ulong8},
                {OpenCL.TypeEnums.DataTypes.Ulong16, from_ulong16},
                {OpenCL.TypeEnums.DataTypes.Long2, from_long2},
                {OpenCL.TypeEnums.DataTypes.Long3, from_long3},
                {OpenCL.TypeEnums.DataTypes.Long4, from_long4},
                {OpenCL.TypeEnums.DataTypes.Long8, from_long8},
                {OpenCL.TypeEnums.DataTypes.Long16, from_long16},

                {OpenCL.TypeEnums.DataTypes.Uint2, from_uint2},
                {OpenCL.TypeEnums.DataTypes.Uint3, from_uint3},
                {OpenCL.TypeEnums.DataTypes.Uint4, from_uint4},
                {OpenCL.TypeEnums.DataTypes.Uint8, from_uint8},
                {OpenCL.TypeEnums.DataTypes.Uint16, from_uint16},
                {OpenCL.TypeEnums.DataTypes.Int2, from_int2},
                {OpenCL.TypeEnums.DataTypes.Int3, from_int3},
                {OpenCL.TypeEnums.DataTypes.Int4, from_int4},
                {OpenCL.TypeEnums.DataTypes.Int8, from_int8},
                {OpenCL.TypeEnums.DataTypes.Int16, from_int16},

                {OpenCL.TypeEnums.DataTypes.Ushort2, from_ushort2},
                {OpenCL.TypeEnums.DataTypes.Ushort3, from_ushort3},
                {OpenCL.TypeEnums.DataTypes.Ushort4, from_short4},
                {OpenCL.TypeEnums.DataTypes.Ushort8, from_ushort8},
                {OpenCL.TypeEnums.DataTypes.Ushort16, from_ushort16},
                {OpenCL.TypeEnums.DataTypes.Short2, from_short2},
                {OpenCL.TypeEnums.DataTypes.Short3, from_short3},
                {OpenCL.TypeEnums.DataTypes.Short4, from_short4},
                {OpenCL.TypeEnums.DataTypes.Short8, from_short8},
                {OpenCL.TypeEnums.DataTypes.Short16, from_short16},

                {OpenCL.TypeEnums.DataTypes.Float2, from_float2},
                {OpenCL.TypeEnums.DataTypes.Float3, from_float3},
                {OpenCL.TypeEnums.DataTypes.Float4, from_float4},
                {OpenCL.TypeEnums.DataTypes.Float8, from_float8},
                {OpenCL.TypeEnums.DataTypes.Float16, from_float16}
            };

        /// <summary>
        /// Converts the Range of a number to a different one
        /// </summary>
        /// <param name="value">The Value to be changed</param>
        /// <param name="oldMax">The old maximum range</param>
        /// <param name="max">the new maximum range</param>
        /// <returns></returns>
        private static float ConvertRange(float value, float oldMax, float max)
        {
            float d = value / oldMax;
            return d * max;
        }

        /// <summary>
        /// Converts the Specified value(that also needs to be a CL struct e.g. uchar4 to float4 is valid) into another CL struct
        /// </summary>
        /// <param name="newType">The new type of the Value</param>
        /// <param name="value">The value</param>
        /// <returns>The value as the new Type</returns>
        public static object Convert(Type newType, object value)
        {
            OpenCL.TypeEnums.DataTypes olddt = KernelParameter.GetEnumFromType(value.GetType());
            OpenCL.TypeEnums.DataTypes dt = KernelParameter.GetEnumFromType(newType);

            string oldName = KernelParameter.GetDataString(olddt);
            string newName = KernelParameter.GetDataString(dt);

            float oldMax = KernelParameter.GetDataMaxSize(oldName);
            float newMax = KernelParameter.GetDataMaxSize(newName);
            int w = ClProgram.GetVectorNum(oldName);
            if (w == 1)
            {
                return System.Convert.ChangeType(
                    ConvertRange((float) System.Convert.ChangeType(value, typeof(float)), oldMax, newMax), newType);
            }

            object[] objs = _fromConverter[olddt](value);


            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = System.Convert.ChangeType(
                    ConvertRange((float) System.Convert.ChangeType(objs[i], typeof(float)), oldMax, newMax),
                    _baseTypes[dt]);
            }

            return _toConverter[dt](objs);
        }

        /// <summary>
        /// Delegate used to Create the ToConverter
        /// </summary>
        /// <param name="args">The Numers</param>
        /// <returns>a CL type</returns>
        private delegate object ConvertToN(object[] args);

        /// <summary>
        /// Delegate used to Create the FromConverter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>An array of numbers resembling the components of the CL Type</returns>
        private delegate object[] ConvertFromN(object value);


        #region Float

        /// <summary>
        /// FromFloat 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_float16(object value)
        {
            float16 val = (float16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromFloat 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_float8(object value)
        {
            float8 val = (float8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromFloat 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_float4(object value)
        {
            float4 val = (float4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromFloat 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_float3(object value)
        {
            float3 val = (float3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromFloat 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_float2(object value)
        {
            float2 val = (float2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }


        /// <summary>
        /// ToFloat 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_float16(params object[] args)
        {
            return new float16((float) args[0], (float) args[1], (float) args[2], (float) args[3], (float) args[4],
                (float) args[5], (float) args[6], (float) args[7], (float) args[8], (float) args[9], (float) args[10],
                (float) args[11], (float) args[12], (float) args[13], (float) args[14], (float) args[15]);
        }


        /// <summary>
        /// ToFloat 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_float8(params object[] args)
        {
            return new float8((float) args[0], (float) args[1], (float) args[2], (float) args[3], (float) args[4],
                (float) args[5], (float) args[6], (float) args[7]);
        }

        /// <summary>
        /// ToFloat 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_float4(params object[] args)
        {
            return new float4((float) args[0], (float) args[1], (float) args[2], (float) args[3]);
        }

        /// <summary>
        /// ToFloat 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_float3(params object[] args)
        {
            return new float3((float) args[0], (float) args[1], (float) args[2]);
        }

        /// <summary>
        /// ToFloat 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_float2(params object[] args)
        {
            return new float2((float) args[0], (float) args[1]);
        }

        #endregion

        #region Byte

        /// <summary>
        /// FromByte 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_byte16(object value)
        {
            uchar16 val = (uchar16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromByte 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_byte8(object value)
        {
            uchar8 val = (uchar8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromByte 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_byte4(object value)
        {
            uchar4 val = (uchar4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromByte 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_byte3(object value)
        {
            uchar3 val = (uchar3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromByte 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_byte2(object value)
        {
            uchar2 val = (uchar2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToByte 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_byte16(params object[] args)
        {
            return new uchar16((byte) args[0], (byte) args[1], (byte) args[2], (byte) args[3], (byte) args[4],
                (byte) args[5], (byte) args[6], (byte) args[7], (byte) args[8], (byte) args[9], (byte) args[10],
                (byte) args[11], (byte) args[12], (byte) args[13], (byte) args[14], (byte) args[15]);
        }

        /// <summary>
        /// ToByte 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_byte8(params object[] args)
        {
            return new uchar8((byte) args[0], (byte) args[1], (byte) args[2], (byte) args[3], (byte) args[4],
                (byte) args[5], (byte) args[6], (byte) args[7]);
        }

        /// <summary>
        /// ToByte 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_byte4(params object[] args)
        {
            return new uchar4((byte) args[0], (byte) args[1], (byte) args[2], (byte) args[3]);
        }

        /// <summary>
        /// ToByte 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_byte3(params object[] args)
        {
            return new uchar3((byte) args[0], (byte) args[1], (byte) args[2]);
        }

        /// <summary>
        /// ToByte 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_byte2(params object[] args)
        {
            return new uchar2((byte) args[0], (byte) args[1]);
        }

        #endregion

        #region SByte

        /// <summary>
        /// FromSbyte 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_sbyte16(object value)
        {
            char16 val = (char16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromSbyte 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_sbyte8(object value)
        {
            char8 val = (char8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromSbyte 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_sbyte4(object value)
        {
            char4 val = (char4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromSbyte 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_sbyte3(object value)
        {
            char3 val = (char3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromSbyte 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_sbyte2(object value)
        {
            char2 val = (char2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }


        /// <summary>
        /// ToSbyte 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_sbyte16(params object[] args)
        {
            return new char16((sbyte) args[0], (sbyte) args[1], (sbyte) args[2], (sbyte) args[3], (sbyte) args[4],
                (sbyte) args[5], (sbyte) args[6], (sbyte) args[7], (sbyte) args[8], (sbyte) args[9], (sbyte) args[10],
                (sbyte) args[11], (sbyte) args[12], (sbyte) args[13], (sbyte) args[14], (sbyte) args[15]);
        }

        /// <summary>
        /// ToSbyte 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_sbyte8(params object[] args)
        {
            return new char8((sbyte) args[0], (sbyte) args[1], (sbyte) args[2], (sbyte) args[3], (sbyte) args[4],
                (sbyte) args[5], (sbyte) args[6], (sbyte) args[7]);
        }

        /// <summary>
        /// ToSbyte 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_sbyte4(params object[] args)
        {
            return new char4((sbyte) args[0], (sbyte) args[1], (sbyte) args[2], (sbyte) args[3]);
        }

        /// <summary>
        /// ToSbyte 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_sbyte3(params object[] args)
        {
            return new char3((sbyte) args[0], (sbyte) args[1], (sbyte) args[2]);
        }

        /// <summary>
        /// ToSbyte 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_sbyte2(params object[] args)
        {
            return new char2((sbyte) args[0], (sbyte) args[1]);
        }

        #endregion

        #region Long

        /// <summary>
        /// FromLong 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_long16(object value)
        {
            long16 val = (long16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromLong 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_long8(object value)
        {
            long8 val = (long8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromLong 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_long4(object value)
        {
            long4 val = (long4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromLong 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_long3(object value)
        {
            long3 val = (long3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromLong 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_long2(object value)
        {
            long2 val = (long2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToLong 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_long16(params object[] args)
        {
            return new long16((long) args[0], (long) args[1], (long) args[2], (long) args[3], (long) args[4],
                (long) args[5], (long) args[6], (long) args[7], (long) args[8], (long) args[9], (long) args[10],
                (long) args[11], (long) args[12], (long) args[13], (long) args[14], (long) args[15]);
        }

        /// <summary>
        /// ToLong 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_long8(params object[] args)
        {
            return new long8((long) args[0], (long) args[1], (long) args[2], (long) args[3], (long) args[4],
                (long) args[5], (long) args[6], (long) args[7]);
        }

        /// <summary>
        /// ToLong 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_long4(params object[] args)
        {
            return new long4((long) args[0], (long) args[1], (long) args[2], (long) args[3]);
        }

        /// <summary>
        /// ToLong 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_long3(params object[] args)
        {
            return new long3((long) args[0], (long) args[1], (long) args[2]);
        }

        /// <summary>
        /// ToLong 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_long2(params object[] args)
        {
            return new long2((long) args[0], (long) args[1]);
        }

        #endregion

        #region ULong

        /// <summary>
        /// FromULong 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ulong16(object value)
        {
            ulong16 val = (ulong16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromULong 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ulong8(object value)
        {
            ulong8 val = (ulong8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromULong 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ulong4(object value)
        {
            ulong4 val = (ulong4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromULong 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ulong3(object value)
        {
            ulong3 val = (ulong3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromULong 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ulong2(object value)
        {
            ulong2 val = (ulong2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToULong 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ulong16(params object[] args)
        {
            return new ulong16((ulong) args[0], (ulong) args[1], (ulong) args[2], (ulong) args[3], (ulong) args[4],
                (ulong) args[5], (ulong) args[6], (ulong) args[7], (ulong) args[8], (ulong) args[9], (ulong) args[10],
                (ulong) args[11], (ulong) args[12], (ulong) args[13], (ulong) args[14], (ulong) args[15]);
        }

        /// <summary>
        /// ToULong 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ulong8(params object[] args)
        {
            return new ulong8((ulong) args[0], (ulong) args[1], (ulong) args[2], (ulong) args[3], (ulong) args[4],
                (ulong) args[5], (ulong) args[6], (ulong) args[7]);
        }

        /// <summary>
        /// ToULong 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ulong4(params object[] args)
        {
            return new ulong4((ulong) args[0], (ulong) args[1], (ulong) args[2], (ulong) args[3]);
        }

        /// <summary>
        /// ToULong 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ulong3(params object[] args)
        {
            return new ulong3((ulong) args[0], (ulong) args[1], (ulong) args[2]);
        }

        /// <summary>
        /// ToULong 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ulong2(params object[] args)
        {
            return new ulong2((ulong) args[0], (ulong) args[1]);
        }

        #endregion

        #region Int

        /// <summary>
        /// FromInt 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_int16(object value)
        {
            int16 val = (int16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromInt 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_int8(object value)
        {
            int8 val = (int8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromInt 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_int4(object value)
        {
            int4 val = (int4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromInt 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_int3(object value)
        {
            int3 val = (int3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromInt 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_int2(object value)
        {
            int2 val = (int2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToInt 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_int16(params object[] args)
        {
            return new int16((int) args[0], (int) args[1], (int) args[2], (int) args[3], (int) args[4], (int) args[5],
                (int) args[6], (int) args[7], (int) args[8], (int) args[9], (int) args[10], (int) args[11],
                (int) args[12], (int) args[13], (int) args[14], (int) args[15]);
        }

        /// <summary>
        /// ToInt 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_int8(params object[] args)
        {
            return new int8((int) args[0], (int) args[1], (int) args[2], (int) args[3], (int) args[4], (int) args[5],
                (int) args[6], (int) args[7]);
        }

        /// <summary>
        /// ToInt 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_int4(params object[] args)
        {
            return new int4((int) args[0], (int) args[1], (int) args[2], (int) args[3]);
        }

        /// <summary>
        /// ToInt 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_int3(params object[] args)
        {
            return new int3((int) args[0], (int) args[1], (int) args[2]);
        }

        /// <summary>
        /// ToInt 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_int2(params object[] args)
        {
            return new int2((int) args[0], (int) args[1]);
        }

        #endregion

        #region UInt

        /// <summary>
        /// FromUInt 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_uint16(object value)
        {
            uint16 val = (uint16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromUInt 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_uint8(object value)
        {
            uint8 val = (uint8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromUInt 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_uint4(object value)
        {
            uint4 val = (uint4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromUInt 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_uint3(object value)
        {
            uint3 val = (uint3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromUInt 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_uint2(object value)
        {
            uint2 val = (uint2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToUInt 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_uint16(params object[] args)
        {
            return new uint16((uint) args[0], (uint) args[1], (uint) args[2], (uint) args[3], (uint) args[4],
                (uint) args[5], (uint) args[6], (uint) args[7], (uint) args[8], (uint) args[9], (uint) args[10],
                (uint) args[11], (uint) args[12], (uint) args[13], (uint) args[14], (uint) args[15]);
        }

        /// <summary>
        /// ToUInt 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_uint8(params object[] args)
        {
            return new uint8((uint) args[0], (uint) args[1], (uint) args[2], (uint) args[3], (uint) args[4],
                (uint) args[5], (uint) args[6], (uint) args[7]);
        }

        /// <summary>
        /// ToUInt 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_uint4(params object[] args)
        {
            return new uint4((uint) args[0], (uint) args[1], (uint) args[2], (uint) args[3]);
        }

        /// <summary>
        /// ToUInt 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_uint3(params object[] args)
        {
            return new uint3((uint) args[0], (uint) args[1], (uint) args[2]);
        }

        /// <summary>
        /// ToUInt 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_uint2(params object[] args)
        {
            return new uint2((uint) args[0], (uint) args[1]);
        }

        #endregion

        #region Short

        /// <summary>
        /// FromShort 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_short16(object value)
        {
            short16 val = (short16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_short8(object value)
        {
            short8 val = (short8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_short4(object value)
        {
            short4 val = (short4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_short3(object value)
        {
            short3 val = (short3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_short2(object value)
        {
            short2 val = (short2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToShort 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_short16(params object[] args)
        {
            return new short16((short) args[0], (short) args[1], (short) args[2], (short) args[3], (short) args[4],
                (short) args[5], (short) args[6], (short) args[7], (short) args[8], (short) args[9], (short) args[10],
                (short) args[11], (short) args[12], (short) args[13], (short) args[14], (short) args[15]);
        }

        /// <summary>
        /// ToShort 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_short8(params object[] args)
        {
            return new short8((short) args[0], (short) args[1], (short) args[2], (short) args[3], (short) args[4],
                (short) args[5], (short) args[6], (short) args[7]);
        }

        /// <summary>
        /// ToShort 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_short4(params object[] args)
        {
            return new short4((short) args[0], (short) args[1], (short) args[2], (short) args[3]);
        }

        /// <summary>
        /// ToShort 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_short3(params object[] args)
        {
            return new short3((short) args[0], (short) args[1], (short) args[2]);
        }

        /// <summary>
        /// ToShort 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_short2(params object[] args)
        {
            return new short2((short) args[0], (short) args[1]);
        }

        #endregion

        #region UShort

        /// <summary>
        /// FromShort 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ushort16(object value)
        {
            ushort16 val = (ushort16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ushort8(object value)
        {
            ushort8 val = (ushort8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ushort4(object value)
        {
            ushort4 val = (ushort4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ushort3(object value)
        {
            ushort3 val = (ushort3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// FromShort 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] from_ushort2(object value)
        {
            ushort2 val = (ushort2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        /// ToUShort 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ushort16(params object[] args)
        {
            return new ushort16((ushort) args[0], (ushort) args[1], (ushort) args[2], (ushort) args[3],
                (ushort) args[4], (ushort) args[5], (ushort) args[6], (ushort) args[7], (ushort) args[8],
                (ushort) args[9], (ushort) args[10], (ushort) args[11], (ushort) args[12], (ushort) args[13],
                (ushort) args[14], (ushort) args[15]);
        }

        /// <summary>
        /// ToUShort 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ushort8(params object[] args)
        {
            return new ushort8((ushort) args[0], (ushort) args[1], (ushort) args[2], (ushort) args[3], (ushort) args[4],
                (ushort) args[5], (ushort) args[6], (ushort) args[7]);
        }

        /// <summary>
        /// ToUShort 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ushort4(params object[] args)
        {
            return new ushort4((ushort) args[0], (ushort) args[1], (ushort) args[2], (ushort) args[3]);
        }

        /// <summary>
        /// ToUShort 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ushort3(params object[] args)
        {
            return new ushort3((ushort) args[0], (ushort) args[1], (ushort) args[2]);
        }

        /// <summary>
        /// ToUShort 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object create_ushort2(params object[] args)
        {
            return new ushort2((ushort) args[0], (ushort) args[1]);
        }

        #endregion
    }
}