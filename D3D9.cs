﻿using System;
using System.Runtime.InteropServices;
using SlimDX.Direct3D9;
using WhiteMagic.Internals;

namespace D3DDetour
{
    public class D3D9 : D3DHook
    {
        private Direct3D9EndScene _endSceneDelegate;
        private Detour _endSceneHook;

        public IntPtr EndScenePointer = IntPtr.Zero;
        public IntPtr ResetPointer = IntPtr.Zero;
        public IntPtr ResetExPointer = IntPtr.Zero;

        const int VMT_ENDSCENE = 42;
        const int VMT_RESET = 16;

        public override void Initialize()
        {
            Device tmpDevice;
            using (var d3d = new Direct3D())
            {
                using (tmpDevice = new Device(d3d, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, new PresentParameters() { BackBufferWidth = 1, BackBufferHeight = 1 }))
                {
                    EndScenePointer = Core.Magic.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_ENDSCENE);
                    ResetPointer = Core.Magic.GetObjectVtableFunction(tmpDevice.ComPointer, VMT_RESET);
                }
            }

            _endSceneDelegate = Core.Magic.RegisterDelegate<Direct3D9EndScene>(EndScenePointer);
            _endSceneHook = Core.Magic.Detours.CreateAndApply(_endSceneDelegate, new Direct3D9EndScene(Callback), "D9EndScene");
        }

        private int Callback(IntPtr device)
        {
            RaiseEvent();
            return (int)_endSceneHook.CallOriginal(device);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Direct3D9Reset(IntPtr device, PresentParameters presentationParameters);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Direct3D9ResetEx(IntPtr presentationParameters, IntPtr displayModeEx);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Direct3D9EndScene(IntPtr device);
    }
}
