#region License and Copyright Notice

// Copyright (c) 2010 Ananth B.
// All rights reserved.
// 
// The contents of this file are made available under the terms of the
// Eclipse Public License v1.0 (the "License") which accompanies this
// distribution, and is available at the following URL:
// http://www.opensource.org/licenses/eclipse-1.0.php
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either expressed or implied. See the License for
// the specific language governing rights and limitations under the License.
// 
// By using this software in any fashion, you are agreeing to be bound by the
// terms of the License.

#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Engine.OpenCL.DotNetCore.DataTypes
{
    using uchar = System.Byte;

    public interface IVectorType
    {
        int Rank { get; }
        IntPtr Size { get; }
    }


    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct char2 : IVectorType
    {
        [FieldOffset(0)] public sbyte x;
        [FieldOffset(0)] public sbyte s0;
        [FieldOffset(1)] public sbyte y;
        [FieldOffset(1)] public sbyte s1;

        public char2(sbyte v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public char2(sbyte x, sbyte y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public sbyte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct char3 : IVectorType
    {
        [FieldOffset(0)] public sbyte x;
        [FieldOffset(0)] public sbyte s0;
        [FieldOffset(1)] public sbyte y;
        [FieldOffset(1)] public sbyte s1;
        [FieldOffset(2)] public sbyte z;
        [FieldOffset(2)] public sbyte s2;
        [FieldOffset(3)] public sbyte w;
        [FieldOffset(3)] public sbyte s3;

        public char3(sbyte v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public char3(sbyte x, sbyte y, sbyte z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public sbyte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct char4 : IVectorType
    {
        [FieldOffset(0)] public sbyte x;
        [FieldOffset(0)] public sbyte s0;
        [FieldOffset(1)] public sbyte y;
        [FieldOffset(1)] public sbyte s1;
        [FieldOffset(2)] public sbyte z;
        [FieldOffset(2)] public sbyte s2;
        [FieldOffset(3)] public sbyte w;
        [FieldOffset(3)] public sbyte s3;

        public char4(sbyte v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public char4(sbyte x, sbyte y, sbyte z, sbyte w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public sbyte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct char8 : IVectorType
    {
        [FieldOffset(0)] public sbyte s0;
        [FieldOffset(1)] public sbyte s1;
        [FieldOffset(2)] public sbyte s2;
        [FieldOffset(3)] public sbyte s3;
        [FieldOffset(4)] public sbyte s4;
        [FieldOffset(5)] public sbyte s5;
        [FieldOffset(6)] public sbyte s6;
        [FieldOffset(7)] public sbyte s7;

        public char8(sbyte v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public char8(sbyte s0, sbyte s1, sbyte s2, sbyte s3, sbyte s4, sbyte s5, sbyte s6, sbyte s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public sbyte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct char16 : IVectorType
    {
        [FieldOffset(0)] public sbyte s0;
        [FieldOffset(1)] public sbyte s1;
        [FieldOffset(2)] public sbyte s2;
        [FieldOffset(3)] public sbyte s3;
        [FieldOffset(4)] public sbyte s4;
        [FieldOffset(5)] public sbyte s5;
        [FieldOffset(6)] public sbyte s6;
        [FieldOffset(7)] public sbyte s7;
        [FieldOffset(8)] public sbyte s8;
        [FieldOffset(9)] public sbyte s9;
        [FieldOffset(10)] public sbyte sa;
        [FieldOffset(10)] public sbyte sA;
        [FieldOffset(11)] public sbyte sb;
        [FieldOffset(11)] public sbyte sB;
        [FieldOffset(12)] public sbyte sc;
        [FieldOffset(12)] public sbyte sC;
        [FieldOffset(13)] public sbyte sd;
        [FieldOffset(13)] public sbyte sD;
        [FieldOffset(14)] public sbyte se;
        [FieldOffset(14)] public sbyte sE;
        [FieldOffset(15)] public sbyte sf;
        [FieldOffset(15)] public sbyte sF;

        public char16(sbyte v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public char16(sbyte s0, sbyte s1, sbyte s2, sbyte s3, sbyte s4, sbyte s5, sbyte s6, sbyte s7, sbyte s8,
            sbyte s9, sbyte sa, sbyte sb, sbyte sc, sbyte sd, sbyte se, sbyte sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public sbyte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct uchar2 : IVectorType
    {
        [FieldOffset(0)] public byte x;
        [FieldOffset(0)] public byte s0;
        [FieldOffset(1)] public byte y;
        [FieldOffset(1)] public byte s1;

        public uchar2(byte v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public uchar2(byte x, byte y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct uchar3 : IVectorType
    {
        [FieldOffset(0)] public byte x;
        [FieldOffset(0)] public byte s0;
        [FieldOffset(1)] public byte y;
        [FieldOffset(1)] public byte s1;
        [FieldOffset(2)] public byte z;
        [FieldOffset(2)] public byte s2;
        [FieldOffset(3)] public byte w;
        [FieldOffset(3)] public byte s3;

        public uchar3(byte v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public uchar3(byte x, byte y, byte z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct uchar4 : IVectorType
    {
        [FieldOffset(0)] public byte x;
        [FieldOffset(0)] public byte s0;
        [FieldOffset(1)] public byte y;
        [FieldOffset(1)] public byte s1;
        [FieldOffset(2)] public byte z;
        [FieldOffset(2)] public byte s2;
        [FieldOffset(3)] public byte w;
        [FieldOffset(3)] public byte s3;

        public uchar4(byte v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public uchar4(byte x, byte y, byte z, byte w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct uchar8 : IVectorType
    {
        [FieldOffset(0)] public byte s0;
        [FieldOffset(1)] public byte s1;
        [FieldOffset(2)] public byte s2;
        [FieldOffset(3)] public byte s3;
        [FieldOffset(4)] public byte s4;
        [FieldOffset(5)] public byte s5;
        [FieldOffset(6)] public byte s6;
        [FieldOffset(7)] public byte s7;

        public uchar8(byte v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public uchar8(byte s0, byte s1, byte s2, byte s3, byte s4, byte s5, byte s6, byte s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct uchar16 : IVectorType
    {
        [FieldOffset(0)] public byte s0;
        [FieldOffset(1)] public byte s1;
        [FieldOffset(2)] public byte s2;
        [FieldOffset(3)] public byte s3;
        [FieldOffset(4)] public byte s4;
        [FieldOffset(5)] public byte s5;
        [FieldOffset(6)] public byte s6;
        [FieldOffset(7)] public byte s7;
        [FieldOffset(8)] public byte s8;
        [FieldOffset(9)] public byte s9;
        [FieldOffset(10)] public byte sa;
        [FieldOffset(10)] public byte sA;
        [FieldOffset(11)] public byte sb;
        [FieldOffset(11)] public byte sB;
        [FieldOffset(12)] public byte sc;
        [FieldOffset(12)] public byte sC;
        [FieldOffset(13)] public byte sd;
        [FieldOffset(13)] public byte sD;
        [FieldOffset(14)] public byte se;
        [FieldOffset(14)] public byte sE;
        [FieldOffset(15)] public byte sf;
        [FieldOffset(15)] public byte sF;

        public uchar16(byte v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public uchar16(byte s0, byte s1, byte s2, byte s3, byte s4, byte s5, byte s6, byte s7, byte s8, byte s9,
            byte sa, byte sb, byte sc, byte sd, byte se, byte sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 1);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct short2 : IVectorType
    {
        [FieldOffset(0)] public short x;
        [FieldOffset(0)] public short s0;
        [FieldOffset(2)] public short y;
        [FieldOffset(2)] public short s1;

        public short2(short v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public short2(short x, short y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct short3 : IVectorType
    {
        [FieldOffset(0)] public short x;
        [FieldOffset(0)] public short s0;
        [FieldOffset(2)] public short y;
        [FieldOffset(2)] public short s1;
        [FieldOffset(4)] public short z;
        [FieldOffset(4)] public short s2;
        [FieldOffset(6)] public short w;
        [FieldOffset(6)] public short s3;

        public short3(short v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public short3(short x, short y, short z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct short4 : IVectorType
    {
        [FieldOffset(0)] public short x;
        [FieldOffset(0)] public short s0;
        [FieldOffset(2)] public short y;
        [FieldOffset(2)] public short s1;
        [FieldOffset(4)] public short z;
        [FieldOffset(4)] public short s2;
        [FieldOffset(6)] public short w;
        [FieldOffset(6)] public short s3;

        public short4(short v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public short4(short x, short y, short z, short w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct short8 : IVectorType
    {
        [FieldOffset(0)] public short s0;
        [FieldOffset(2)] public short s1;
        [FieldOffset(4)] public short s2;
        [FieldOffset(6)] public short s3;
        [FieldOffset(8)] public short s4;
        [FieldOffset(10)] public short s5;
        [FieldOffset(12)] public short s6;
        [FieldOffset(14)] public short s7;

        public short8(short v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public short8(short s0, short s1, short s2, short s3, short s4, short s5, short s6, short s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct short16 : IVectorType
    {
        [FieldOffset(0)] public short s0;
        [FieldOffset(2)] public short s1;
        [FieldOffset(4)] public short s2;
        [FieldOffset(6)] public short s3;
        [FieldOffset(8)] public short s4;
        [FieldOffset(10)] public short s5;
        [FieldOffset(12)] public short s6;
        [FieldOffset(14)] public short s7;
        [FieldOffset(16)] public short s8;
        [FieldOffset(18)] public short s9;
        [FieldOffset(20)] public short sa;
        [FieldOffset(20)] public short sA;
        [FieldOffset(22)] public short sb;
        [FieldOffset(22)] public short sB;
        [FieldOffset(24)] public short sc;
        [FieldOffset(24)] public short sC;
        [FieldOffset(26)] public short sd;
        [FieldOffset(26)] public short sD;
        [FieldOffset(28)] public short se;
        [FieldOffset(28)] public short sE;
        [FieldOffset(30)] public short sf;
        [FieldOffset(30)] public short sF;

        public short16(short v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public short16(short s0, short s1, short s2, short s3, short s4, short s5, short s6, short s7, short s8,
            short s9, short sa, short sb, short sc, short sd, short se, short sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct ushort2 : IVectorType
    {
        [FieldOffset(0)] public ushort x;
        [FieldOffset(0)] public ushort s0;
        [FieldOffset(2)] public ushort y;
        [FieldOffset(2)] public ushort s1;

        public ushort2(ushort v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public ushort2(ushort x, ushort y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct ushort3 : IVectorType
    {
        [FieldOffset(0)] public ushort x;
        [FieldOffset(0)] public ushort s0;
        [FieldOffset(2)] public ushort y;
        [FieldOffset(2)] public ushort s1;
        [FieldOffset(4)] public ushort z;
        [FieldOffset(4)] public ushort s2;
        [FieldOffset(6)] public ushort w;
        [FieldOffset(6)] public ushort s3;

        public ushort3(ushort v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public ushort3(ushort x, ushort y, ushort z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct ushort4 : IVectorType
    {
        [FieldOffset(0)] public ushort x;
        [FieldOffset(0)] public ushort s0;
        [FieldOffset(2)] public ushort y;
        [FieldOffset(2)] public ushort s1;
        [FieldOffset(4)] public ushort z;
        [FieldOffset(4)] public ushort s2;
        [FieldOffset(6)] public ushort w;
        [FieldOffset(6)] public ushort s3;

        public ushort4(ushort v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public ushort4(ushort x, ushort y, ushort z, ushort w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct ushort8 : IVectorType
    {
        [FieldOffset(0)] public ushort s0;
        [FieldOffset(2)] public ushort s1;
        [FieldOffset(4)] public ushort s2;
        [FieldOffset(6)] public ushort s3;
        [FieldOffset(8)] public ushort s4;
        [FieldOffset(10)] public ushort s5;
        [FieldOffset(12)] public ushort s6;
        [FieldOffset(14)] public ushort s7;

        public ushort8(ushort v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public ushort8(ushort s0, ushort s1, ushort s2, ushort s3, ushort s4, ushort s5, ushort s6, ushort s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct ushort16 : IVectorType
    {
        [FieldOffset(0)] public ushort s0;
        [FieldOffset(2)] public ushort s1;
        [FieldOffset(4)] public ushort s2;
        [FieldOffset(6)] public ushort s3;
        [FieldOffset(8)] public ushort s4;
        [FieldOffset(10)] public ushort s5;
        [FieldOffset(12)] public ushort s6;
        [FieldOffset(14)] public ushort s7;
        [FieldOffset(16)] public ushort s8;
        [FieldOffset(18)] public ushort s9;
        [FieldOffset(20)] public ushort sa;
        [FieldOffset(20)] public ushort sA;
        [FieldOffset(22)] public ushort sb;
        [FieldOffset(22)] public ushort sB;
        [FieldOffset(24)] public ushort sc;
        [FieldOffset(24)] public ushort sC;
        [FieldOffset(26)] public ushort sd;
        [FieldOffset(26)] public ushort sD;
        [FieldOffset(28)] public ushort se;
        [FieldOffset(28)] public ushort sE;
        [FieldOffset(30)] public ushort sf;
        [FieldOffset(30)] public ushort sF;

        public ushort16(ushort v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public ushort16(ushort s0, ushort s1, ushort s2, ushort s3, ushort s4, ushort s5, ushort s6, ushort s7,
            ushort s8, ushort s9, ushort sa, ushort sb, ushort sc, ushort sd, ushort se, ushort sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 2);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct int2 : IVectorType
    {
        [FieldOffset(0)] public int x;
        [FieldOffset(0)] public int s0;
        [FieldOffset(4)] public int y;
        [FieldOffset(4)] public int s1;

        public int2(int v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public int2(int x, int y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct int3 : IVectorType
    {
        [FieldOffset(0)] public int x;
        [FieldOffset(0)] public int s0;
        [FieldOffset(4)] public int y;
        [FieldOffset(4)] public int s1;
        [FieldOffset(8)] public int z;
        [FieldOffset(8)] public int s2;
        [FieldOffset(12)] public int w;
        [FieldOffset(12)] public int s3;

        public int3(int v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public int3(int x, int y, int z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct int4 : IVectorType
    {
        [FieldOffset(0)] public int x;
        [FieldOffset(0)] public int s0;
        [FieldOffset(4)] public int y;
        [FieldOffset(4)] public int s1;
        [FieldOffset(8)] public int z;
        [FieldOffset(8)] public int s2;
        [FieldOffset(12)] public int w;
        [FieldOffset(12)] public int s3;

        public int4(int v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public int4(int x, int y, int z, int w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct int8 : IVectorType
    {
        [FieldOffset(0)] public int s0;
        [FieldOffset(4)] public int s1;
        [FieldOffset(8)] public int s2;
        [FieldOffset(12)] public int s3;
        [FieldOffset(16)] public int s4;
        [FieldOffset(20)] public int s5;
        [FieldOffset(24)] public int s6;
        [FieldOffset(28)] public int s7;

        public int8(int v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public int8(int s0, int s1, int s2, int s3, int s4, int s5, int s6, int s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct int16 : IVectorType
    {
        [FieldOffset(0)] public int s0;
        [FieldOffset(4)] public int s1;
        [FieldOffset(8)] public int s2;
        [FieldOffset(12)] public int s3;
        [FieldOffset(16)] public int s4;
        [FieldOffset(20)] public int s5;
        [FieldOffset(24)] public int s6;
        [FieldOffset(28)] public int s7;
        [FieldOffset(32)] public int s8;
        [FieldOffset(36)] public int s9;
        [FieldOffset(40)] public int sa;
        [FieldOffset(40)] public int sA;
        [FieldOffset(44)] public int sb;
        [FieldOffset(44)] public int sB;
        [FieldOffset(48)] public int sc;
        [FieldOffset(48)] public int sC;
        [FieldOffset(52)] public int sd;
        [FieldOffset(52)] public int sD;
        [FieldOffset(56)] public int se;
        [FieldOffset(56)] public int sE;
        [FieldOffset(60)] public int sf;
        [FieldOffset(60)] public int sF;

        public int16(int v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public int16(int s0, int s1, int s2, int s3, int s4, int s5, int s6, int s7, int s8, int s9, int sa, int sb,
            int sc, int sd, int se, int sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct uint2 : IVectorType
    {
        [FieldOffset(0)] public uint x;
        [FieldOffset(0)] public uint s0;
        [FieldOffset(4)] public uint y;
        [FieldOffset(4)] public uint s1;

        public uint2(uint v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public uint2(uint x, uint y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct uint3 : IVectorType
    {
        [FieldOffset(0)] public uint x;
        [FieldOffset(0)] public uint s0;
        [FieldOffset(4)] public uint y;
        [FieldOffset(4)] public uint s1;
        [FieldOffset(8)] public uint z;
        [FieldOffset(8)] public uint s2;
        [FieldOffset(12)] public uint w;
        [FieldOffset(12)] public uint s3;

        public uint3(uint v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public uint3(uint x, uint y, uint z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct uint4 : IVectorType
    {
        [FieldOffset(0)] public uint x;
        [FieldOffset(0)] public uint s0;
        [FieldOffset(4)] public uint y;
        [FieldOffset(4)] public uint s1;
        [FieldOffset(8)] public uint z;
        [FieldOffset(8)] public uint s2;
        [FieldOffset(12)] public uint w;
        [FieldOffset(12)] public uint s3;

        public uint4(uint v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public uint4(uint x, uint y, uint z, uint w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct uint8 : IVectorType
    {
        [FieldOffset(0)] public uint s0;
        [FieldOffset(4)] public uint s1;
        [FieldOffset(8)] public uint s2;
        [FieldOffset(12)] public uint s3;
        [FieldOffset(16)] public uint s4;
        [FieldOffset(20)] public uint s5;
        [FieldOffset(24)] public uint s6;
        [FieldOffset(28)] public uint s7;

        public uint8(uint v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public uint8(uint s0, uint s1, uint s2, uint s3, uint s4, uint s5, uint s6, uint s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct uint16 : IVectorType
    {
        [FieldOffset(0)] public uint s0;
        [FieldOffset(4)] public uint s1;
        [FieldOffset(8)] public uint s2;
        [FieldOffset(12)] public uint s3;
        [FieldOffset(16)] public uint s4;
        [FieldOffset(20)] public uint s5;
        [FieldOffset(24)] public uint s6;
        [FieldOffset(28)] public uint s7;
        [FieldOffset(32)] public uint s8;
        [FieldOffset(36)] public uint s9;
        [FieldOffset(40)] public uint sa;
        [FieldOffset(40)] public uint sA;
        [FieldOffset(44)] public uint sb;
        [FieldOffset(44)] public uint sB;
        [FieldOffset(48)] public uint sc;
        [FieldOffset(48)] public uint sC;
        [FieldOffset(52)] public uint sd;
        [FieldOffset(52)] public uint sD;
        [FieldOffset(56)] public uint se;
        [FieldOffset(56)] public uint sE;
        [FieldOffset(60)] public uint sf;
        [FieldOffset(60)] public uint sF;

        public uint16(uint v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public uint16(uint s0, uint s1, uint s2, uint s3, uint s4, uint s5, uint s6, uint s7, uint s8, uint s9, uint sa,
            uint sb, uint sc, uint sd, uint se, uint sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public uint this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct long2 : IVectorType
    {
        [FieldOffset(0)] public long x;
        [FieldOffset(0)] public long s0;
        [FieldOffset(8)] public long y;
        [FieldOffset(8)] public long s1;

        public long2(long v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public long2(long x, long y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct long3 : IVectorType
    {
        [FieldOffset(0)] public long x;
        [FieldOffset(0)] public long s0;
        [FieldOffset(8)] public long y;
        [FieldOffset(8)] public long s1;
        [FieldOffset(16)] public long z;
        [FieldOffset(16)] public long s2;
        [FieldOffset(24)] public long w;
        [FieldOffset(24)] public long s3;

        public long3(long v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public long3(long x, long y, long z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct long4 : IVectorType
    {
        [FieldOffset(0)] public long x;
        [FieldOffset(0)] public long s0;
        [FieldOffset(8)] public long y;
        [FieldOffset(8)] public long s1;
        [FieldOffset(16)] public long z;
        [FieldOffset(16)] public long s2;
        [FieldOffset(24)] public long w;
        [FieldOffset(24)] public long s3;

        public long4(long v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public long4(long x, long y, long z, long w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct long8 : IVectorType
    {
        [FieldOffset(0)] public long s0;
        [FieldOffset(8)] public long s1;
        [FieldOffset(16)] public long s2;
        [FieldOffset(24)] public long s3;
        [FieldOffset(32)] public long s4;
        [FieldOffset(40)] public long s5;
        [FieldOffset(48)] public long s6;
        [FieldOffset(56)] public long s7;

        public long8(long v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public long8(long s0, long s1, long s2, long s3, long s4, long s5, long s6, long s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct long16 : IVectorType
    {
        [FieldOffset(0)] public long s0;
        [FieldOffset(8)] public long s1;
        [FieldOffset(16)] public long s2;
        [FieldOffset(24)] public long s3;
        [FieldOffset(32)] public long s4;
        [FieldOffset(40)] public long s5;
        [FieldOffset(48)] public long s6;
        [FieldOffset(56)] public long s7;
        [FieldOffset(64)] public long s8;
        [FieldOffset(72)] public long s9;
        [FieldOffset(80)] public long sa;
        [FieldOffset(80)] public long sA;
        [FieldOffset(88)] public long sb;
        [FieldOffset(88)] public long sB;
        [FieldOffset(96)] public long sc;
        [FieldOffset(96)] public long sC;
        [FieldOffset(104)] public long sd;
        [FieldOffset(104)] public long sD;
        [FieldOffset(112)] public long se;
        [FieldOffset(112)] public long sE;
        [FieldOffset(120)] public long sf;
        [FieldOffset(120)] public long sF;

        public long16(long v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public long16(long s0, long s1, long s2, long s3, long s4, long s5, long s6, long s7, long s8, long s9, long sa,
            long sb, long sc, long sd, long se, long sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct ulong2 : IVectorType
    {
        [FieldOffset(0)] public ulong x;
        [FieldOffset(0)] public ulong s0;
        [FieldOffset(8)] public ulong y;
        [FieldOffset(8)] public ulong s1;

        public ulong2(ulong v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public ulong2(ulong x, ulong y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public ulong this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct ulong3 : IVectorType
    {
        [FieldOffset(0)] public ulong x;
        [FieldOffset(0)] public ulong s0;
        [FieldOffset(8)] public ulong y;
        [FieldOffset(8)] public ulong s1;
        [FieldOffset(16)] public ulong z;
        [FieldOffset(16)] public ulong s2;
        [FieldOffset(24)] public ulong w;
        [FieldOffset(24)] public ulong s3;

        public ulong3(ulong v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public ulong3(ulong x, ulong y, ulong z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public ulong this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct ulong4 : IVectorType
    {
        [FieldOffset(0)] public ulong x;
        [FieldOffset(0)] public ulong s0;
        [FieldOffset(8)] public ulong y;
        [FieldOffset(8)] public ulong s1;
        [FieldOffset(16)] public ulong z;
        [FieldOffset(16)] public ulong s2;
        [FieldOffset(24)] public ulong w;
        [FieldOffset(24)] public ulong s3;

        public ulong4(ulong v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public ulong4(ulong x, ulong y, ulong z, ulong w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public ulong this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct ulong8 : IVectorType
    {
        [FieldOffset(0)] public ulong s0;
        [FieldOffset(8)] public ulong s1;
        [FieldOffset(16)] public ulong s2;
        [FieldOffset(24)] public ulong s3;
        [FieldOffset(32)] public ulong s4;
        [FieldOffset(40)] public ulong s5;
        [FieldOffset(48)] public ulong s6;
        [FieldOffset(56)] public ulong s7;

        public ulong8(ulong v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public ulong8(ulong s0, ulong s1, ulong s2, ulong s3, ulong s4, ulong s5, ulong s6, ulong s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public ulong this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct ulong16 : IVectorType
    {
        [FieldOffset(0)] public ulong s0;
        [FieldOffset(8)] public ulong s1;
        [FieldOffset(16)] public ulong s2;
        [FieldOffset(24)] public ulong s3;
        [FieldOffset(32)] public ulong s4;
        [FieldOffset(40)] public ulong s5;
        [FieldOffset(48)] public ulong s6;
        [FieldOffset(56)] public ulong s7;
        [FieldOffset(64)] public ulong s8;
        [FieldOffset(72)] public ulong s9;
        [FieldOffset(80)] public ulong sa;
        [FieldOffset(80)] public ulong sA;
        [FieldOffset(88)] public ulong sb;
        [FieldOffset(88)] public ulong sB;
        [FieldOffset(96)] public ulong sc;
        [FieldOffset(96)] public ulong sC;
        [FieldOffset(104)] public ulong sd;
        [FieldOffset(104)] public ulong sD;
        [FieldOffset(112)] public ulong se;
        [FieldOffset(112)] public ulong sE;
        [FieldOffset(120)] public ulong sf;
        [FieldOffset(120)] public ulong sF;

        public ulong16(ulong v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public ulong16(ulong s0, ulong s1, ulong s2, ulong s3, ulong s4, ulong s5, ulong s6, ulong s7, ulong s8,
            ulong s9, ulong sa, ulong sb, ulong sc, ulong sd, ulong se, ulong sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public ulong this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct float2 : IVectorType
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(0)] public float s0;
        [FieldOffset(4)] public float y;
        [FieldOffset(4)] public float s1;

        public float2(float v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public float2(float x, float y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct float3 : IVectorType
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(0)] public float s0;
        [FieldOffset(4)] public float y;
        [FieldOffset(4)] public float s1;
        [FieldOffset(8)] public float z;
        [FieldOffset(8)] public float s2;
        [FieldOffset(12)] public float w;
        [FieldOffset(12)] public float s3;

        public float3(float v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public float3(float x, float y, float z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct float4 : IVectorType
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(0)] public float s0;
        [FieldOffset(4)] public float y;
        [FieldOffset(4)] public float s1;
        [FieldOffset(8)] public float z;
        [FieldOffset(8)] public float s2;
        [FieldOffset(12)] public float w;
        [FieldOffset(12)] public float s3;

        public float4(float v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public float4(float x, float y, float z, float w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct float8 : IVectorType
    {
        [FieldOffset(0)] public float s0;
        [FieldOffset(4)] public float s1;
        [FieldOffset(8)] public float s2;
        [FieldOffset(12)] public float s3;
        [FieldOffset(16)] public float s4;
        [FieldOffset(20)] public float s5;
        [FieldOffset(24)] public float s6;
        [FieldOffset(28)] public float s7;

        public float8(float v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public float8(float s0, float s1, float s2, float s3, float s4, float s5, float s6, float s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct float16 : IVectorType
    {
        [FieldOffset(0)] public float s0;
        [FieldOffset(4)] public float s1;
        [FieldOffset(8)] public float s2;
        [FieldOffset(12)] public float s3;
        [FieldOffset(16)] public float s4;
        [FieldOffset(20)] public float s5;
        [FieldOffset(24)] public float s6;
        [FieldOffset(28)] public float s7;
        [FieldOffset(32)] public float s8;
        [FieldOffset(36)] public float s9;
        [FieldOffset(40)] public float sa;
        [FieldOffset(40)] public float sA;
        [FieldOffset(44)] public float sb;
        [FieldOffset(44)] public float sB;
        [FieldOffset(48)] public float sc;
        [FieldOffset(48)] public float sC;
        [FieldOffset(52)] public float sd;
        [FieldOffset(52)] public float sD;
        [FieldOffset(56)] public float se;
        [FieldOffset(56)] public float sE;
        [FieldOffset(60)] public float sf;
        [FieldOffset(60)] public float sF;

        public float16(float v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public float16(float s0, float s1, float s2, float s3, float s4, float s5, float s6, float s7, float s8,
            float s9, float sa, float sb, float sc, float sd, float se, float sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 4);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}")]
    public struct double2 : IVectorType
    {
        [FieldOffset(0)] public double x;
        [FieldOffset(0)] public double s0;
        [FieldOffset(8)] public double y;
        [FieldOffset(8)] public double s1;

        public double2(double v)
        {
            x = s0 = v;
            y = s1 = v;
        }

        public double2(double x, double y)
        {
            this.x = s0 = x;
            this.y = s1 = y;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 2;
        public IntPtr Size => (IntPtr) (2 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1}", x, y);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct double3 : IVectorType
    {
        [FieldOffset(0)] public double x;
        [FieldOffset(0)] public double s0;
        [FieldOffset(8)] public double y;
        [FieldOffset(8)] public double s1;
        [FieldOffset(16)] public double z;
        [FieldOffset(16)] public double s2;
        [FieldOffset(24)] public double w;
        [FieldOffset(24)] public double s3;

        public double3(double v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public double3(double x, double y, double z)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            w = s3 = 0;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 3;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("x={x}, y={y}, z={z}, w={w}")]
    public struct double4 : IVectorType
    {
        [FieldOffset(0)] public double x;
        [FieldOffset(0)] public double s0;
        [FieldOffset(8)] public double y;
        [FieldOffset(8)] public double s1;
        [FieldOffset(16)] public double z;
        [FieldOffset(16)] public double s2;
        [FieldOffset(24)] public double w;
        [FieldOffset(24)] public double s3;

        public double4(double v)
        {
            x = s0 = v;
            y = s1 = v;
            z = s2 = v;
            w = s3 = v;
        }

        public double4(double x, double y, double z, double w)
        {
            this.x = s0 = x;
            this.y = s1 = y;
            this.z = s2 = z;
            this.w = s3 = w;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 4;
        public IntPtr Size => (IntPtr) (4 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", x, y, z, w);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay("s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}")]
    public struct double8 : IVectorType
    {
        [FieldOffset(0)] public double s0;
        [FieldOffset(8)] public double s1;
        [FieldOffset(16)] public double s2;
        [FieldOffset(24)] public double s3;
        [FieldOffset(32)] public double s4;
        [FieldOffset(40)] public double s5;
        [FieldOffset(48)] public double s6;
        [FieldOffset(56)] public double s7;

        public double8(double v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
        }

        public double8(double s0, double s1, double s2, double s3, double s4, double s5, double s6, double s7)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 8;
        public IntPtr Size => (IntPtr) (8 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", s0, s1, s2, s3, s4, s5, s6, s7);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [DebuggerDisplay(
        "s0={s0}, s1={s1}, s2={s2}, s3={s3}, s4={s4}, s5={s5}, s6={s6}, s7={s7}, s8={s8}, s9={s9}, sa={sa}, sb={sb}, sc={sc}, sd={sd}, se={se}, sf={sf}")]
    public struct double16 : IVectorType
    {
        [FieldOffset(0)] public double s0;
        [FieldOffset(8)] public double s1;
        [FieldOffset(16)] public double s2;
        [FieldOffset(24)] public double s3;
        [FieldOffset(32)] public double s4;
        [FieldOffset(40)] public double s5;
        [FieldOffset(48)] public double s6;
        [FieldOffset(56)] public double s7;
        [FieldOffset(64)] public double s8;
        [FieldOffset(72)] public double s9;
        [FieldOffset(80)] public double sa;
        [FieldOffset(80)] public double sA;
        [FieldOffset(88)] public double sb;
        [FieldOffset(88)] public double sB;
        [FieldOffset(96)] public double sc;
        [FieldOffset(96)] public double sC;
        [FieldOffset(104)] public double sd;
        [FieldOffset(104)] public double sD;
        [FieldOffset(112)] public double se;
        [FieldOffset(112)] public double sE;
        [FieldOffset(120)] public double sf;
        [FieldOffset(120)] public double sF;

        public double16(double v)
        {
            s0 = v;
            s1 = v;
            s2 = v;
            s3 = v;
            s4 = v;
            s5 = v;
            s6 = v;
            s7 = v;
            s8 = v;
            s9 = v;
            sa = sA = v;
            sb = sB = v;
            sc = sC = v;
            sd = sD = v;
            se = sE = v;
            sf = sF = v;
        }

        public double16(double s0, double s1, double s2, double s3, double s4, double s5, double s6, double s7,
            double s8, double s9, double sa, double sb, double sc, double sd, double se, double sf)
        {
            this.s0 = s0;
            this.s1 = s1;
            this.s2 = s2;
            this.s3 = s3;
            this.s4 = s4;
            this.s5 = s5;
            this.s6 = s6;
            this.s7 = s7;
            this.s8 = s8;
            this.s9 = s9;
            this.sa = sA = sa;
            this.sb = sB = sb;
            this.sc = sC = sc;
            this.sd = sD = sd;
            this.se = sE = se;
            this.sf = sF = sf;
        }

        #region IVectorType implementation

        int IVectorType.Rank => 16;
        public IntPtr Size => (IntPtr) (16 * 8);

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", s0, s1, s2,
                s3, s4, s5, s6, s7, s8, s9, sa, sb, sc, sd, se, sf);
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return s0;
                    case 1: return s1;
                    case 2: return s2;
                    case 3: return s3;
                    case 4: return s4;
                    case 5: return s5;
                    case 6: return s6;
                    case 7: return s7;
                    case 8: return s8;
                    case 9: return s9;
                    case 10: return sa;
                    case 11: return sb;
                    case 12: return sc;
                    case 13: return sd;
                    case 14: return se;
                    case 15: return sf;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        s0 = value;
                        break;
                    case 1:
                        s1 = value;
                        break;
                    case 2:
                        s2 = value;
                        break;
                    case 3:
                        s3 = value;
                        break;
                    case 4:
                        s4 = value;
                        break;
                    case 5:
                        s5 = value;
                        break;
                    case 6:
                        s6 = value;
                        break;
                    case 7:
                        s7 = value;
                        break;
                    case 8:
                        s8 = value;
                        break;
                    case 9:
                        s9 = value;
                        break;
                    case 10:
                        sa = value;
                        break;
                    case 11:
                        sb = value;
                        break;
                    case 12:
                        sc = value;
                        break;
                    case 13:
                        sd = value;
                        break;
                    case 14:
                        se = value;
                        break;
                    case 15:
                        sf = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }
}