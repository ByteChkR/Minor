using System;
using System.Collections.Generic;
using CLHelperLibrary.CLStructs;
using OpenCl.DotNetCore.DataTypes;
using OpenCl.DotNetCore.Memory;

namespace CLHelperLibrary
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
        public DataTypes DataType { get; set; }

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

        private static List<Tuple<string, DataTypes>> DataTypePairs = new List<Tuple<string, DataTypes>>
        {
            new Tuple<string, DataTypes>( "float", DataTypes.FLOAT1),
            new Tuple<string, DataTypes>( "float2", DataTypes.FLOAT2),
			new Tuple<string, DataTypes>( "float3", DataTypes.FLOAT3),
			new Tuple<string, DataTypes>( "float4", DataTypes.FLOAT4),
			new Tuple<string, DataTypes>( "float8", DataTypes.FLOAT8),
			new Tuple<string, DataTypes>( "float16", DataTypes.FLOAT16),
			new Tuple<string, DataTypes>( "int", DataTypes.INT1),
			new Tuple<string, DataTypes>( "int2", DataTypes.INT2),
			new Tuple<string, DataTypes>( "int3", DataTypes.INT3),
			new Tuple<string, DataTypes>( "int4", DataTypes.INT4),
			new Tuple<string, DataTypes>( "int8", DataTypes.INT8),
			new Tuple<string, DataTypes>( "int16", DataTypes.INT16),
			new Tuple<string, DataTypes>( "uint", DataTypes.UINT1),
			new Tuple<string, DataTypes>( "uint2", DataTypes.UINT2),
			new Tuple<string, DataTypes>( "uint3", DataTypes.UINT3),
			new Tuple<string, DataTypes>( "uint4", DataTypes.UINT4),
			new Tuple<string, DataTypes>( "uint8", DataTypes.UINT8),
			new Tuple<string, DataTypes>( "uint16", DataTypes.UINT16),
			new Tuple<string, DataTypes>( "char", DataTypes.CHAR1),
			new Tuple<string, DataTypes>( "char2", DataTypes.CHAR2),
			new Tuple<string, DataTypes>( "char3", DataTypes.CHAR3),
			new Tuple<string, DataTypes>( "char4", DataTypes.CHAR4),
			new Tuple<string, DataTypes>( "char8", DataTypes.CHAR8),
			new Tuple<string, DataTypes>( "char16", DataTypes.CHAR16),
			new Tuple<string, DataTypes>( "uchar", DataTypes.UCHAR1),
			new Tuple<string, DataTypes>( "uchar2", DataTypes.UCHAR2),
			new Tuple<string, DataTypes>( "uchar3", DataTypes.UCHAR3),
			new Tuple<string, DataTypes>( "uchar4", DataTypes.UCHAR4),
			new Tuple<string, DataTypes>( "uchar8", DataTypes.UCHAR8),
			new Tuple<string, DataTypes>( "uchar16", DataTypes.UCHAR16),
			new Tuple<string, DataTypes>( "short", DataTypes.SHORT1),
			new Tuple<string, DataTypes>( "short2", DataTypes.SHORT2),
			new Tuple<string, DataTypes>( "short3", DataTypes.SHORT3),
			new Tuple<string, DataTypes>( "short4", DataTypes.SHORT4),
			new Tuple<string, DataTypes>( "short8", DataTypes.SHORT8),
			new Tuple<string, DataTypes>( "short16", DataTypes.SHORT16),
			new Tuple<string, DataTypes>( "ushort", DataTypes.USHORT1),
			new Tuple<string, DataTypes>( "ushort2", DataTypes.USHORT2),
			new Tuple<string, DataTypes>( "ushort3", DataTypes.USHORT3),
			new Tuple<string, DataTypes>( "ushort4", DataTypes.USHORT4),
			new Tuple<string, DataTypes>( "ushort8", DataTypes.USHORT8),
			new Tuple<string, DataTypes>( "ushort16", DataTypes.USHORT16),
			new Tuple<string, DataTypes>( "long", DataTypes.LONG1),
			new Tuple<string, DataTypes>( "long2", DataTypes.LONG2),
			new Tuple<string, DataTypes>( "long3", DataTypes.LONG3),
			new Tuple<string, DataTypes>( "long4", DataTypes.LONG4),
			new Tuple<string, DataTypes>( "long8", DataTypes.LONG8),
			new Tuple<string, DataTypes>( "long16", DataTypes.LONG16),
			new Tuple<string, DataTypes>( "ulong1", DataTypes.ULONG1),
			new Tuple<string, DataTypes>( "ulong2", DataTypes.ULONG2),
			new Tuple<string, DataTypes>( "ulong3", DataTypes.ULONG3),
			new Tuple<string, DataTypes>( "ulong4", DataTypes.ULONG4),
			new Tuple<string, DataTypes>( "ulong8", DataTypes.ULONG8),
			new Tuple<string, DataTypes>( "ulong16", DataTypes.ULONG16),
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
                object[] data = (object[])value;

                return CL.CreateBuffer(Array.ConvertAll(data, x => CastToType(Converters[(int)DataType], x)),
                    Converters[(int)DataType], MemoryFlag.CopyHostPointer | MemoryFlag.ReadOnly);

            }
            return CastToType(Converters[(int)DataType], value);
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
            string kernelHeader = code.Substring(startIndex, endIndex);
            int start = kernelHeader.IndexOf('('), end = kernelHeader.LastIndexOf(')');
            string parameters = kernelHeader.Substring(start + 1, (end - start) - 1);
            string[] pars = parameters.Split(',');
            KernelParameter[] ret = new KernelParameter[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                string[] parametr = pars[i].Trim().Split(' ');

                ret[i] = new KernelParameter
                {
                    Name = parametr[parametr.Length - 1].Replace('*', ' ').Trim(),
                    DataType = GetDataType(parametr[parametr.Length - 2].Replace('*', ' ').Trim()),
                    MemScope = GetMemoryScope(parametr.Length == 3 ? parametr[0] : ""),
                    IsArray = (parametr[parametr.Length - 2].IndexOf("*", StringComparison.InvariantCulture) != -1 ||
                               parametr[parametr.Length - 1].IndexOf("*", StringComparison.InvariantCulture) != -1),
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
        public static string GetDataString(DataTypes type)
        {
            foreach (var dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item2==type)
                {
                    return dataTypePair.Item1;
                }
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// returns the Correct DataType enum for the equivalent in OpenCL C99
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DataTypes GetDataType(string str)
        {
            foreach (var dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item1 == str)
                {
                    return dataTypePair.Item2;
                }
            }

            return DataTypes.UNKNOWN;
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