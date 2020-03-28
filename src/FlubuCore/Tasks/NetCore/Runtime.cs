using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure;

namespace FlubuCore.Tasks.NetCore
{
    public class Runtime : Enumeration
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        public static readonly Runtime WinX64 = new Runtime(1, "win-x64");
        public static readonly Runtime WinX86 = new Runtime(2, "win-x86");
        public static readonly Runtime WinArm = new Runtime(3, "win-arm");
        public static readonly Runtime WinArm64 = new Runtime(4, "win-arm64");
        public static readonly Runtime Win7X64 = new Runtime(5, "win7-x64");
        public static readonly Runtime Win7X86 = new Runtime(6, "win7-x86");
        public static readonly Runtime Win81X64 = new Runtime(7, "win81-x64");
        public static readonly Runtime Win81X86 = new Runtime(8, "win81-x86");
        public static readonly Runtime Win81Arm = new Runtime(9, "win81-arm");
        public static readonly Runtime Win10X64 = new Runtime(10, "win10-x64");
        public static readonly Runtime Win10X86 = new Runtime(11, "win10-x86");
        public static readonly Runtime Win10Arm = new Runtime(12, "win10-arm");
        public static readonly Runtime Win10Arm64 = new Runtime(13, "win10-arm64");

        public static readonly Runtime LinuxX64 = new Runtime(14, "linux-x64");
        public static readonly Runtime LinuxMuslX64 = new Runtime(15, "linux-musl-x64");
        public static readonly Runtime LinuxArmX64 = new Runtime(16, "linux-arm");
        public static readonly Runtime RhelX64 = new Runtime(17, "rhel-x64");
        public static readonly Runtime Rhel_6X64 = new Runtime(18, "rhel.6-x64");
        public static readonly Runtime Tizien = new Runtime(19, "tizien");
        public static readonly Runtime Tizien_4_0_0 = new Runtime(20, "tizien.4.0.0");
        public static readonly Runtime Tizien_5_0_0 = new Runtime(21, "tizien.5.0.0");

        public static readonly Runtime Osx_x64 = new Runtime(22, "osx-x64");
        public static readonly Runtime Osx_10_10_x64 = new Runtime(23, "osx.10.10-x64");
        public static readonly Runtime Osx_10_11_x64 = new Runtime(24, "osx.10.11-x64");
        public static readonly Runtime Osx_10_12_x64 = new Runtime(25, "osx.10.12-x64");
        public static readonly Runtime Osx_10_13_x64 = new Runtime(26, "osx.10.13-x64");
        public static readonly Runtime Osx_10_14_x64 = new Runtime(27, "osx.10.14-x64");

#pragma warning restore SA1310 // Field names should not contain underscore

        public Runtime(int id, string name)
            : base(id, name)
        {
        }
    }
}
