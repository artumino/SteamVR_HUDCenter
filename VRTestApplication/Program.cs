using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using SteamVR_HUDCenter;
using Valve.VR;
using OpenTK;
using SteamVR_HUDCenter.Elements;

namespace VRTestApplication
{
    class Program
    {
        public static CVROverlay overlay;
        public static CVRSystem hmd;
        public static CVRCompositor compositor;
        public static SteamVR vr;
        public static ulong overlayHandle = 0;
        public static ulong dashOverlayHandle = 1;

        public static bool _IsRunning = false;

        [STAThread]
        static void Main(string[] args)
        {
            GameWindow window = new GameWindow(300, 300);
            
            vr = SteamVR.instance;

            EVRInitError error = EVRInitError.None;

            OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);

            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing OpenVR!");
            }

            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing Compositor!");
            }

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
            {
                throw new Exception("An error occured while initializing Overlay!");
            }

            hmd = OpenVR.System;
            compositor = OpenVR.Compositor;
            overlay = OpenVR.Overlay;

            MainOverlay dashOverlay = new MainOverlay("Test", @"./Resources/hl3.jpg", 2.0f, VROverlayInputMethod.Mouse);
            MainOverlay handOverlay = new MainOverlay("handOverlay", 1.0f);
            handOverlay.SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole.LeftHand);
            
            System.Threading.Thread OverlayThread = new System.Threading.Thread(new System.Threading.ThreadStart(OverlayCycle));
            OverlayThread.IsBackground = true;
            OverlayThread.Start();
            Console.ReadLine();
            _IsRunning = false;
        }

        public static void OverlayCycle()
        {
            _IsRunning = true;
            while (_IsRunning)
            {
                foreach(Handlable overlay in OpenVR_Utils.RegisteredItems)
                    if(overlay is Overlay)
                        HandleVRInput((Overlay)overlay);
                System.Threading.Thread.Sleep(20);
            }
        }

        public static void HandleVRInput(Overlay Overlay)
        {
            for (uint unDeviceId = 1; unDeviceId < OpenVR.k_unControllerStateAxisCount; unDeviceId++)
            {
                if (overlay.HandleControllerOverlayInteractionAsMouse(Overlay.Handle, unDeviceId))
                {
                    break;
                }
            }

            VREvent_t vrEvent = new VREvent_t();
            while (overlay.PollNextOverlayEvent(Overlay.Handle, ref vrEvent, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t))))
            {
                switch (vrEvent.eventType)
                {
                    case (int)EVREventType.VREvent_MouseMove:
                        Overlay.RaiseOnVREvent_MouseMove(vrEvent.data);
                        break;
                    case (int)EVREventType.VREvent_MouseButtonDown:
                        Overlay.RaiseOnVREvent_MouseButtonDown(vrEvent.data);
                        ToggleNonDashOverlay();
                        break;
                    case (int)EVREventType.VREvent_MouseButtonUp:
                        Overlay.RaiseOnVREvent_MouseButtonUp(vrEvent.data);
                        break;
                    case (int)EVREventType.VREvent_OverlayShown:
                        Overlay.RaiseOnVREvent_OverlayShown(vrEvent.data);
                        break;
                    case (int)EVREventType.VREvent_Quit:
                        Overlay.RaiseOnVREvent_Quit(vrEvent.data);
                        break;
                }
            }
        }

        public static bool _isVisible = true;
        public static void ToggleNonDashOverlay()
        {
            if (_isVisible)
                overlay.HideOverlay(overlayHandle);
            else
                overlay.ShowOverlay(overlayHandle);
            _isVisible = !_isVisible;
        }
    }
}
