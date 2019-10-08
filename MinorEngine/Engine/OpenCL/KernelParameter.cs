using System;
using System.Collections.Generic;
using Engine.OpenCL.TypeEnums;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.Memory;

namespace Engine.OpenCL
{
    /// <summary>
    /// A class containing the logic to parse a kernel argument
    /// </summary>
    public class KernelParameter
    {
        /// <summary>
        /// The name of the argument
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Data type
        /// </summary>
        public TypeEnums.DataTypes DataType { get; set; }

        /// <summary>
        /// Is the Argument an Array?
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// The Argument id of the Parameter
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The scope of the argument
        /// </summary>
        public MemoryScope MemScope { get; set; }

        /// <summary>
        /// A list of Types(in the same order as the DataType enum
        /// </summary>
        private static Type[] Converters => new[]
        {
            typeof(object),
            typeof(float),
            typeof(float2),
            typeof(float3),
            typeof(float4),
            typeof(float8),
            typeof(float16),
            typeof(int),
            typeof(int2),
            typeof(int3),
            typeof(int4),
            typeof(int8),
            typeof(int16),
            typeof(uint),
            typeof(uint2),
            typeof(uint3),
            typeof(uint4),
            typeof(uint8),
            typeof(uint16),
            typeof(sbyte),
            typeof(char2),
            typeof(char3),
            typeof(char4),
            typeof(char8),
            typeof(char16),
            typeof(byte),
            typeof(uchar2),
            typeof(uchar3),
            typeof(uchar4),
            typeof(uchar8),
            typeof(uchar16),
            typeof(short),
            typeof(short2),
            typeof(short3),
            typeof(short4),
            typeof(short8),
            typeof(short16),
            typeof(ushort),
            typeof(ushort2),
            typeof(ushort3),
            typeof(ushort4),
            typeof(ushort8),
            typeof(ushort16),
            typeof(long),
            typeof(long2),
            typeof(long3),
            typeof(long4),
            typeof(long8),
            typeof(long16),
            typeof(ulong),
            typeof(ulong2),
            typeof(ulong3),
            typeof(ulong4),
            typeof(ulong8),
            typeof(ulong16)
        };

        private static List<Tuple<string, float, TypeEnums.DataTypes>> DataTypePairs = new List<Tuple<string, float, TypeEnums.DataTypes>>
        {
            new Tuple<string, float, TypeEnums.DataTypes>("float", float.MaxValue, TypeEnums.DataTypes.FLOAT1),
            new Tuple<string, float, TypeEnums.DataTypes>("float2", float.MaxValue, TypeEnums.DataTypes.FLOAT2),
            new Tuple<string, float, TypeEnums.DataTypes>("float3", float.MaxValue, TypeEnums.DataTypes.FLOAT3),
            new Tuple<string, float, TypeEnums.DataTypes>("float4", float.MaxValue, TypeEnums.DataTypes.FLOAT4),
            new Tuple<string, float, TypeEnums.DataTypes>("float8", float.MaxValue, TypeEnums.DataTypes.FLOAT8),
            new Tuple<string, float, TypeEnums.DataTypes>("float16", float.MaxValue, TypeEnums.DataTypes.FLOAT16),
            new Tuple<string, float, TypeEnums.DataTypes>("int", int.MaxValue, TypeEnums.DataTypes.INT1),
            new Tuple<string, float, TypeEnums.DataTypes>("int2", int.MaxValue, TypeEnums.DataTypes.INT2),
            new Tuple<string, float, TypeEnums.DataTypes>("int3", int.MaxValue, TypeEnums.DataTypes.INT3),
            new Tuple<string, float, TypeEnums.DataTypes>("int4", int.MaxValue, TypeEnums.DataTypes.INT4),
            new Tuple<string, float, TypeEnums.DataTypes>("int8", int.MaxValue, TypeEnums.DataTypes.INT8),
            new Tuple<string, float, TypeEnums.DataTypes>("int16", int.MaxValue, TypeEnums.DataTypes.INT16),
            new Tuple<string, float, TypeEnums.DataTypes>("uint", uint.MaxValue, TypeEnums.DataTypes.UINT1),
            new Tuple<string, float, TypeEnums.DataTypes>("uint2", uint.MaxValue, TypeEnums.DataTypes.UINT2),
            new Tuple<string, float, TypeEnums.DataTypes>("uint3", uint.MaxValue, TypeEnums.DataTypes.UINT3),
            new Tuple<string, float, TypeEnums.DataTypes>("uint4", uint.MaxValue, TypeEnums.DataTypes.UINT4),
            new Tuple<string, float, TypeEnums.DataTypes>("uint8", uint.MaxValue, TypeEnums.DataTypes.UINT8),
            new Tuple<string, float, TypeEnums.DataTypes>("uint16", uint.MaxValue, TypeEnums.DataTypes.UINT16),
            new Tuple<string, float, TypeEnums.DataTypes>("char", sbyte.MaxValue, TypeEnums.DataTypes.CHAR1),
            new Tuple<string, float, TypeEnums.DataTypes>("char2", sbyte.MaxValue, TypeEnums.DataTypes.CHAR2),
            new Tuple<string, float, TypeEnums.DataTypes>("char3", sbyte.MaxValue, TypeEnums.DataTypes.CHAR3),
            new Tuple<string, float, TypeEnums.DataTypes>("char4", sbyte.MaxValue, TypeEnums.DataTypes.CHAR4),
            new Tuple<string, float, TypeEnums.DataTypes>("char8", sbyte.MaxValue, TypeEnums.DataTypes.CHAR8),
            new Tuple<string, float, TypeEnums.DataTypes>("char16", sbyte.MaxValue, TypeEnums.DataTypes.CHAR16),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar", byte.MaxValue, TypeEnums.DataTypes.UCHAR1),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar2", byte.MaxValue, TypeEnums.DataTypes.UCHAR2),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar3", byte.MaxValue, TypeEnums.DataTypes.UCHAR3),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar4", byte.MaxValue, TypeEnums.DataTypes.UCHAR4),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar8", byte.MaxValue, TypeEnums.DataTypes.UCHAR8),
            new Tuple<string, float, TypeEnums.DataTypes>("uchar16", byte.MaxValue, TypeEnums.DataTypes.UCHAR16),
            new Tuple<string, float, TypeEnums.DataTypes>("short", short.MaxValue, TypeEnums.DataTypes.SHORT1),
            new Tuple<string, float, TypeEnums.DataTypes>("short2", short.MaxValue, TypeEnums.DataTypes.SHORT2),
            new Tuple<string, float, TypeEnums.DataTypes>("short3", short.MaxValue, TypeEnums.DataTypes.SHORT3),
            new Tuple<string, float, TypeEnums.DataTypes>("short4", short.MaxValue, TypeEnums.DataTypes.SHORT4),
            new Tuple<string, float, TypeEnums.DataTypes>("short8", short.MaxValue, TypeEnums.DataTypes.SHORT8),
            new Tuple<string, float, TypeEnums.DataTypes>("short16", short.MaxValue, TypeEnums.DataTypes.SHORT16),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort", ushort.MaxValue, TypeEnums.DataTypes.USHORT1),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort2", ushort.MaxValue, TypeEnums.DataTypes.USHORT2),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort3", ushort.MaxValue, TypeEnums.DataTypes.USHORT3),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort4", ushort.MaxValue, TypeEnums.DataTypes.USHORT4),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort8", ushort.MaxValue, TypeEnums.DataTypes.USHORT8),
            new Tuple<string, float, TypeEnums.DataTypes>("ushort16", ushort.MaxValue, TypeEnums.DataTypes.USHORT16),
            new Tuple<string, float, TypeEnums.DataTypes>("long", long.MaxValue, TypeEnums.DataTypes.LONG1),
            new Tuple<string, float, TypeEnums.DataTypes>("long2", long.MaxValue, TypeEnums.DataTypes.LONG2),
            new Tuple<string, float, TypeEnums.DataTypes>("long3", long.MaxValue, TypeEnums.DataTypes.LONG3),
            new Tuple<string, float, TypeEnums.DataTypes>("long4", long.MaxValue, TypeEnums.DataTypes.LONG4),
            new Tuple<string, float, TypeEnums.DataTypes>("long8", long.MaxValue, TypeEnums.DataTypes.LONG8),
            new Tuple<string, float, TypeEnums.DataTypes>("long16", long.MaxValue, TypeEnums.DataTypes.LONG16),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong", ulong.MaxValue, TypeEnums.DataTypes.ULONG1),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong2", ulong.MaxValue, TypeEnums.DataTypes.ULONG2),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong3", ulong.MaxValue, TypeEnums.DataTypes.ULONG3),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong4", ulong.MaxValue, TypeEnums.DataTypes.ULONG4),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong8", ulong.MaxValue, TypeEnums.DataTypes.ULONG8),
            new Tuple<string, float, TypeEnums.DataTypes>("ulong16", ulong.MaxValue, TypeEnums.DataTypes.ULONG16)
        };

        /// <summary>
        /// Casts the supplied value to the specified type
        /// </summary>
        /// <param name="value">the value casted to the required type for the parameter</param>
        /// <returns></returns>
        public object CastToType(object value)
        {
            if (IsArray)
            {
                var data = (object[]) value;

                return CLAPI.CreateBuffer(Array.ConvertAll(data, x => CastToType(Converters[(int) DataType], x)),
                    Converters[(int) DataType], MemoryFlag.CopyHostPointer | MemoryFlag.ReadOnly);
            }

            return CastToType(Converters[(int) DataType], value);
        }

        /// <summary>
        /// Casts the supplied value to type t
        /// </summary>
        /// <param name="t">the target type</param>
        /// <param name="value">the value to be casted</param>
        /// <returns>The casted value</returns>
        private static object CastToType(Type t, object value)
        {
            return Convert.ChangeType(value, t);
        }


        /// <summary>
        /// Parses the kernel parameters from the kernel signature
        /// </summary>
        /// <param name="code">The full program code</param>
        /// <param name="startIndex">The index where the kernel name starts</param>
        /// <param name="endIndex">the index after the bracket of the signature closed</param>
        /// <returns>A parsed list of Kernel parameters</returns>
        public static KernelParameter[] CreateKernelParametersFromKernelCode(string code, int startIndex, int endIndex)
        {
            var kernelHeader = code.Substring(startIndex, endIndex);
            int start = kernelHeader.IndexOf('('), end = kernelHeader.LastIndexOf(')');
            var parameters = kernelHeader.Substring(start + 1, end - start - 1);
            var pars = parameters.Split(',');
            var ret = new KernelParameter[pars.Length];
            for (var i = 0; i < pars.Length; i++)
            {
                var parametr = pars[i].Trim().Split(' ');

                ret[i] = new KernelParameter
                {
                    Name = parametr[parametr.Length - 1].Replace('*', ' ').Trim(),
                    DataType = GetDataType(parametr[parametr.Length - 2].Replace('*', ' ').Trim()),
                    MemScope = GetMemoryScope(parametr.Length == 3 ? parametr[0] : ""),
                    IsArray = parametr[parametr.Length - 2].IndexOf("*", StringComparison.InvariantCulture) != -1 ||
                              parametr[parametr.Length - 1].IndexOf("*", StringComparison.InvariantCulture) != -1,
                    Id = i
                };
            }

            return ret;
        }

        /// <summary>
        /// returns the Correct DataType string for the equivalent in the CL Library
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDataString(TypeEnums.DataTypes type)
        {
            foreach (var dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item3 == type)
                {
                    return dataTypePair.Item1;
                }
            }

            return "UNKNOWN";
        }

        public static float GetDataMaxSize(string genType)
        {
            foreach (var dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item1 == genType)
                {
                    return dataTypePair.Item2;
                }
            }

            return 0;
        }

        /// <summary>
        /// returns the Correct DataType enum for the equivalent in OpenCL C99
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeEnums.DataTypes GetDataType(string str)
        {
            foreach (var dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item1 == str)
                {
                    return dataTypePair.Item3;
                }
            }

            return TypeEnums.DataTypes.UNKNOWN;
        }

        /// <summary>
        /// Returns the memory scope that is associated with the modifier
        /// </summary>
        /// <param name="modifier">The modifier to be tested</param>
        /// <returns>the MemoryScope</returns>
        private static MemoryScope GetMemoryScope(string modifier)
        {
            switch (modifier)
            {
                case "__global":
                    return MemoryScope.GLOBAL;
                case "global":
                    return MemoryScope.GLOBAL;
                default:
                    return MemoryScope.NONE;
            }
        }
    }
}