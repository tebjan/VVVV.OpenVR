using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils;
using Valve.VR;
using VVVV.Utils.VMath;
using SlimDX;

namespace VVVV.Nodes.ValveOpenVR
{
    //the convention horror
    public static class OpenVRExtensions
    {
        public static Matrix ToProjectionMatrix(this HmdMatrix44_t m)
        {
            return new Matrix()
            {
                M11 = m.m0,
                M12 = m.m4,
                M13 = m.m8,
                M14 = m.m12,

                M21 = m.m1,
                M22 = m.m5,
                M23 = m.m9,
                M24 = m.m13,

                M31 = -m.m2,
                M32 = -m.m6,
                M33 = -m.m10,
                M34 = -m.m14,

                M41 = m.m3,
                M42 = m.m7,
                M43 = m.m11,
                M44 = m.m15
            };
        }

        public static Matrix ToEyeMatrix(this HmdMatrix34_t m)
        {
            return new Matrix()
            {
                M11 = m.m0,
                M12 = m.m4,
                M13 = m.m8,
                M14 = 0,

                M21 = m.m1,
                M22 = m.m5,
                M23 = m.m9,
                M24 = 0,

                M31 = m.m2,
                M32 = m.m6,
                M33 = m.m10,
                M34 = 0,

                M41 = m.m3,
                M42 = m.m7,
                M43 = -m.m11,
                M44 = 1
            };
        }

        public static Matrix ToMatrix(this HmdMatrix34_t m)
        {
            return new Matrix()
            {
                M11 = m.m0,
                M12 = m.m4,
                M13 = -m.m8,
                M14 = 0,

                M21 = m.m1,
                M22 = m.m5,
                M23 = -m.m9,
                M24 = 0,

                M31 = -m.m2,
                M32 = -m.m6,
                M33 = m.m10,
                M34 = 0,

                M41 = m.m3,
                M42 = m.m7,
                M43 = -m.m11,
                M44 = 1
            };
        }

        public static Vector3 Pos(this Matrix m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }
    }
}
