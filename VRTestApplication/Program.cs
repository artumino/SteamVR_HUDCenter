using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using SteamVR_HUDCenter;
using Valve.VR;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

            // Non-dashboard overlay
            EVROverlayError overlayError = overlay.CreateOverlay("overlayTest", "HL3", ref overlayHandle);

            // Dashboard overlay
            ulong thumbnailHandle = 3;
            overlay.SetOverlayFromFile(thumbnailHandle, @"./Resources/hl3.jpg");
            EVROverlayError overlayErrorDash = overlay.CreateDashboardOverlay("dashOverlayTest", "HL3", ref dashOverlayHandle, ref thumbnailHandle);

            if (overlayError != EVROverlayError.None)
            {
                throw new Exception(overlayError.ToString());
            }

            if (overlayErrorDash != EVROverlayError.None)
            {
                throw new Exception(overlayErrorDash.ToString());
            }

            // Set overlay parameters
            overlay.SetOverlayWidthInMeters(overlayHandle, 1f);
            overlay.SetOverlayWidthInMeters(dashOverlayHandle, 1f);

            // Non-dashboard overlay stuff
            HmdMatrix34_t nmatrix = OTK_Utils.OpenTKMatrixToOpenVRMatrix(new Matrix3x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0)
            ));

            // Gets left controller index without hard-conding it
            overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, hmd.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand), ref nmatrix);
            overlay.SetOverlayInputMethod(overlayHandle, VROverlayInputMethod.Mouse);
            overlay.SetOverlayInputMethod(dashOverlayHandle, VROverlayInputMethod.Mouse);

            Bitmap bmp = new Bitmap(@"./Resources/hl3.jpg");
            int textureID = GL.GenTexture();

            System.Drawing.Imaging.BitmapData TextureData =
            bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            bmp.UnlockBits(TextureData);

            Texture_t texture = new Texture_t();
            texture.eType = EGraphicsAPIConvention.API_OpenGL;
            texture.eColorSpace = EColorSpace.Auto;
            texture.handle = (IntPtr)textureID;
            overlay.SetOverlayTexture(overlayHandle, ref texture);
            overlay.SetOverlayTexture(dashOverlayHandle, ref texture);

            overlay.ShowOverlay(overlayHandle);

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
                HandleVRInput(dashOverlayHandle);
                System.Threading.Thread.Sleep(20);
            }
        }

        public static void HandleVRInput(ulong Overlay)
        {
            for (uint unDeviceId = 1; unDeviceId < OpenVR.k_unControllerStateAxisCount; unDeviceId++)
            {
                if (overlay.HandleControllerOverlayInteractionAsMouse(Overlay, unDeviceId))
                {
                    break;
                }
            }

            VREvent_t vrEvent = new VREvent_t();
            while (overlay.PollNextOverlayEvent(Overlay, ref vrEvent, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t))))
            {
                switch (vrEvent.eventType)
                {
                    case (int)EVREventType.VREvent_MouseMove:
                        UDebug.Log("Mouse Move!");
                        break;

                    case (int)EVREventType.VREvent_MouseButtonDown:
                        UDebug.Log("Mouse Button Down!");
                        ToggleNonDashOverlay();
                        break;

                    case (int)EVREventType.VREvent_MouseButtonUp:
                        UDebug.Log("Mouse Button Up!");
                        break;
                    case (int)EVREventType.VREvent_OverlayShown:
                        UDebug.Log("Overlay Shown!");
                        break;
                    case (int)EVREventType.VREvent_Quit:
                        _IsRunning = false;
                        System.Environment.Exit(0);
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
