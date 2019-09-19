using System;
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

                return CL.CreateBuffer(Array.ConvertAll(data, x => CastToType(Converters[(int) DataType], x)),
                    Converters[(int) DataType], MemoryFlag.CopyHostPointer | MemoryFlag.ReadOnly);

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
        /// returns the Correct DataType enum for the equivalent in OpenCL C99
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DataTypes GetDataType(string type)
        {
            DataTypes dt;
            switch (type)
            {
                case "float":
                    dt = DataTypes.FLOAT1;
                    break;
                case "float2":
                    dt = DataTypes.FLOAT2;
                    break;
                case "float3":
                    dt = DataTypes.FLOAT3;
                    break;
                case "float4":
                    dt = DataTypes.FLOAT4;
                    break;
                case "float8":
                    dt = DataTypes.FLOAT8;
                    break;
                case "float16":
                    dt = DataTypes.FLOAT16;
                    break;
                case "int":
                    dt = DataTypes.INT1;
                    break;
                case "int2":
                    dt = DataTypes.INT2;
                    break;
                case "int3":
                    dt = DataTypes.INT3;
                    break;
                case "int4":
                    dt = DataTypes.INT4;
                    break;
                case "int8":
                    dt = DataTypes.INT8;
                    break;
                case "int16":
                    dt = DataTypes.INT16;
                    break;
                case "uint":
                    dt = DataTypes.UINT1;
                    break;
                case "uint2":
                    dt = DataTypes.UINT2;
                    break;
                case "uint3":
                    dt = DataTypes.UINT3;
                    break;
                case "uint4":
                    dt = DataTypes.UINT4;
                    break;
                case "uint8":
                    dt = DataTypes.UINT8;
                    break;
                case "uint16":
                    dt = DataTypes.UINT16;
                    break;
                case "char":
                    dt = DataTypes.CHAR1;
                    break;
                case "char2":
                    dt = DataTypes.CHAR2;
                    break;
                case "char3":
                    dt = DataTypes.CHAR3;
                    break;
                case "char4":
                    dt = DataTypes.CHAR4;
                    break;
                case "char8":
                    dt = DataTypes.CHAR8;
                    break;
                case "char16":
                    dt = DataTypes.CHAR16;
                    break;
                case "uchar":
                    dt = DataTypes.UCHAR1;
                    break;
                case "uchar2":
                    dt = DataTypes.UCHAR2;
                    break;
                case "uchar3":
                    dt = DataTypes.UCHAR3;
                    break;
                case "uchar4":
                    dt = DataTypes.UCHAR4;
                    break;
                case "uchar8":
                    dt = DataTypes.UCHAR8;
                    break;
                case "uchar16":
                    dt = DataTypes.UCHAR16;
                    break;
                case "short":
                    dt = DataTypes.SHORT1;
                    break;
                case "short2":
                    dt = DataTypes.SHORT2;
                    break;
                case "short3":
                    dt = DataTypes.SHORT3;
                    break;
                case "short4":
                    dt = DataTypes.SHORT4;
                    break;
                case "short8":
                    dt = DataTypes.SHORT8;
                    break;
                case "short16":
                    dt = DataTypes.SHORT16;
                    break;
                case "ushort":
                    dt = DataTypes.USHORT1;
                    break;
                case "ushort2":
                    dt = DataTypes.USHORT2;
                    break;
                case "ushort3":
                    dt = DataTypes.USHORT3;
                    break;
                case "ushort4":
                    dt = DataTypes.USHORT4;
                    break;
                case "ushort8":
                    dt = DataTypes.USHORT8;
                    break;
                case "ushort16":
                    dt = DataTypes.USHORT16;
                    break;
                case "long":
                    dt = DataTypes.LONG1;
                    break;
                case "long2":
                    dt = DataTypes.LONG2;
                    break;
                case "long3":
                    dt = DataTypes.LONG3;
                    break;
                case "long4":
                    dt = DataTypes.LONG4;
                    break;
                case "long8":
                    dt = DataTypes.LONG8;
                    break;
                case "long16":
                    dt = DataTypes.LONG16;
                    break;
                case "ulong1":
                    dt = DataTypes.ULONG1;
                    break;
                case "ulong2":
                    dt = DataTypes.ULONG2;
                    break;
                case "ulong3":
                    dt = DataTypes.ULONG3;
                    break;
                case "ulong4":
                    dt = DataTypes.ULONG4;
                    break;
                case "ulong8":
                    dt = DataTypes.ULONG8;
                    break;
                case "ulong16":
                    dt = DataTypes.ULONG16;
                    break;
                default:
                    dt = DataTypes.UNKNOWN;
                    break;
            }

            return dt;


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