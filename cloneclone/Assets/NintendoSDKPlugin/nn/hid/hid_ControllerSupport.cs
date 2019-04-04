/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nn.hid
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerSupportArg
    {
        private const int ExplainTextSize = ControllerSupport.ExplainTextMaxBufferSize * 4;

        public byte playerCountMin;
        public byte playerCountMax;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableTakeOverConnection;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableLeftJustify;
        [MarshalAs(UnmanagedType.U1)]
        public bool enablePermitJoyDual;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableSingleMode;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableIdentificationColor;
        public Color4u8Array4 identificationColor;
        [MarshalAs(UnmanagedType.I1)]
        public bool enableExplainText;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ExplainTextSize)]
        private byte[] explainText;

        public void SetDefault()
        {
            this.playerCountMin = 0;
            this.playerCountMax = 4;
            this.enableTakeOverConnection = true;
            this.enableLeftJustify = true;
            this.enablePermitJoyDual = true;
            this.enableSingleMode = false;
            this.enableIdentificationColor = false;
            this.identificationColor = new Color4u8Array4();
            this.enableExplainText = false;
            this.explainText = new byte[ExplainTextSize];
        }

        public override string ToString()
        {
            return string.Format("Min{0} Max{1} TOC{2} LJ{3} PJD{4} SM{5} IC{6} C0{7} C1{8} C2{9} C3{10} ET{11}",
                this.playerCountMin, this.playerCountMax, this.enableTakeOverConnection, this.enableLeftJustify,
                this.enablePermitJoyDual, this.enableSingleMode, this.enableIdentificationColor,
                this.identificationColor[0], this.identificationColor[1],
                this.identificationColor[2], this.identificationColor[3],
                this.enableExplainText);
        }

        #region nn.util.Color4u8Array4
        [StructLayout(LayoutKind.Sequential)]
        public struct Color4u8Array4 : IList<nn.util.Color4u8>, ICollection<nn.util.Color4u8>, IEnumerable<nn.util.Color4u8>
        {
            private const int _Length = 4;
            public int Length { get { return _Length; } }

            private nn.util.Color4u8 _value0;
            private nn.util.Color4u8 _value1;
            private nn.util.Color4u8 _value2;
            private nn.util.Color4u8 _value3;

            public nn.util.Color4u8 this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
                        case 1: return _value1;
                        case 2: return _value2;
                        case 3: return _value3;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            _value0 = value;
                            break;
                        case 1:
                            _value1 = value;
                            break;
                        case 2:
                            _value2 = value;
                            break;
                        case 3:
                            _value3 = value;
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(nn.util.Color4u8 item)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (this[i] == item)
                    {
                        return true;
                    }
                }
                return false;
            }

            public int IndexOf(nn.util.Color4u8 item)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (this[i] == item)
                    {
                        return i;
                    }
                }
                return -1;
            }
            public void CopyTo(nn.util.Color4u8[] array, int arrayIndex)
            {
                if (array == null) { throw new ArgumentNullException(); }
                if (arrayIndex < 0) { throw new ArgumentOutOfRangeException(); }
                if (arrayIndex + Length < array.Length) { throw new ArgumentException(); }
                for (int i = 0; i < Length; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }

            public override string ToString()
            {
                return string.Format("{{{0},{1},{2},{3}}}",
                    _value0, _value1, _value2, _value3);
            }

            public IEnumerator<nn.util.Color4u8> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public bool Remove(nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerFirmwareUpdateArg
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool enableForceUpdate;

        private byte _padding0; 
        private byte _padding1; 
        private byte _padding2;

        public void SetDefault()
        {
            this.enableForceUpdate = false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerSupportResultInfo
    {
        public byte playerCount;
        public NpadId selectedId;

        private byte _padding0;
        private byte _padding1;
        private byte _padding2;

        public override string ToString()
        {
            return string.Format("{0} {1}", this.playerCount, this.selectedId);
        }
    }

    public static partial class ControllerSupport
    {
        public const int ExplainTextMaxLength = 32;
        public const int Utf8ByteSize = 4;
        public const int ExplainTextMaxBufferSize = ExplainTextMaxLength * Utf8ByteSize + 1;

#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show(ControllerSupportArg showControllerSupportArg)
        {
            return new Result();
        }

        public static Result Show(ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg)
        {
            return new Result();
        }

        public static void SetExplainText(ref ControllerSupportArg pOutControllerSupportArg, string pStr, NpadId npadId)
        {
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerSupport")]
        public static extern Result Show(ControllerSupportArg showControllerSupportArg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerSupport2")]
        public static extern Result Show(ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetExplainText")]
        public static extern void SetExplainText(ref ControllerSupportArg pOutControllerSupportArg, string pStr, NpadId npadId);
#endif

        public static Result Show(ControllerSupportArg showControllerSupportArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(showControllerSupportArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(showControllerSupportArg);
        }

        public static Result Show(
            ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(ref pOutValue, showControllerSupportArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(ref pOutValue, showControllerSupportArg);
        }
    }

    public static class ControllerStrapGuide
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show()
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerStrapGuide")]
        public static extern Result Show();
#endif

        public static Result Show(bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show();
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show();
        }
    }

    public static partial class ControllerFirmwareUpdate
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg)
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerFirmwareUpdate")]
        public static extern Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg);
#endif

        public static Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(showControllerFirmwareUpdateArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(showControllerFirmwareUpdateArg);
        }
    }
}
#endif
