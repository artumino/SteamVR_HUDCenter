using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Valve.VR;
using OpenTK;

namespace SteamVR_HUDCenter.Elements
{
    public abstract class Overlay : Handlable
    {
        public VROverlayInputMethod InputMethod { get; private set; }
        public float Width { get; private set; }
        public bool IsDashboardWidget { get; private set; }
        public Handlable Thumbnail { get; private set; }
        public bool IsVisible { get; private set; }

        public delegate void VREvent(VREvent_Data_t Data);
        public event VREvent OnVREvent_MouseMove;
        public event VREvent OnVREvent_MouseButtonDown;
        public event VREvent OnVREvent_MouseButtonUp;
        public event VREvent OnVREvent_OverlayShown;
        public event VREvent OnVREvent_Quit;

        protected HmdMatrix34_t nmatrix = OTK_Utils.OpenTKMatrixToOpenVRMatrix(new Matrix3x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0)
        ));

        public Overlay(string FriendlyName,
            float Width,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None)
            : base(FriendlyName) //Registers Item Handle
        {
            Init(false, "", Width, InputMethod);
        }

        public Overlay(string FriendlyName,
            string ThumbnailPath,
            float Width,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None) 
            : base(FriendlyName) //Registers Item Handle
        {
            Init(true, ThumbnailPath, Width, InputMethod);
        }

        public void Init(bool IsDashboardWidget = true,
            string ThumbnailPath = null,
            float Width = 1.0f,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None)
        {
            this.IsDashboardWidget = IsDashboardWidget;


            EVROverlayError overlayError;
            if (this.IsDashboardWidget)
            {
                //If this is a Dashboard Widget, Create a Thumbnail item
                Thumbnail = new Handlable(this.Name + "_Thumbnail");
                OpenVR.Overlay.SetOverlayFromFile(Thumbnail.Handle, ThumbnailPath);
                overlayError = OpenVR.Overlay.CreateDashboardOverlay(this.Key, this.Name, ref this.Handle, ref this.Thumbnail.Handle);
            }
            else
                overlayError = OpenVR.Overlay.CreateOverlay(this.Key, this.Name, ref this.Handle);

            //Check if there's any error while initializing the overlay
            if (overlayError != EVROverlayError.None)
                throw new Exception(overlayError.ToString());

            SetOverlaySize(Width);
            SetOverlayInputMethod(InputMethod);
            Start();
        }

        #region UtilityMethods
        public void SetOverlaySize(float Width)
        {
            this.Width = Width;
            OpenVR.Overlay.SetOverlayWidthInMeters(this.Handle, Width);
        }

        public void SetOverlayInputMethod(VROverlayInputMethod InputMethod)
        {
            this.InputMethod = InputMethod;
            OpenVR.Overlay.SetOverlayInputMethod(this.Handle, InputMethod);
        }

        public void SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole Device)
        {
            OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(this.Handle, OpenVR.System.GetTrackedDeviceIndexForControllerRole(Device), ref nmatrix);
        }

        public void SetOverlayTransformTrackedDeviceRelative(uint DeviceIndex)
        {
            OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(this.Handle, DeviceIndex, ref nmatrix);
        }

        public void Show()
        {
            this.IsVisible = true;
            OpenVR.Overlay.ShowOverlay(this.Handle);
        }

        public void Hide()
        {
            this.IsVisible = false;
            OpenVR.Overlay.HideOverlay(this.Handle);
        }

        public void ToggleVisibility()
        {
            if(this.IsVisible)
                OpenVR.Overlay.HideOverlay(this.Handle);
            else
                OpenVR.Overlay.ShowOverlay(this.Handle);
            this.IsVisible = !this.IsVisible;
        }
        #endregion

        #region Events
        public void RaiseOnVREvent_MouseMove(VREvent_Data_t Data)
        {
            if (OnVREvent_MouseMove != null)
                OnVREvent_MouseMove.Invoke(Data);
        }

        public void RaiseOnVREvent_MouseButtonDown(VREvent_Data_t Data)
        {
            if (OnVREvent_MouseButtonDown != null)
                OnVREvent_MouseButtonDown.Invoke(Data);
        }

        public void RaiseOnVREvent_MouseButtonUp(VREvent_Data_t Data)
        {
            if (OnVREvent_MouseButtonUp != null)
                OnVREvent_MouseButtonUp.Invoke(Data);
        }

        public void RaiseOnVREvent_OverlayShown(VREvent_Data_t Data)
        {
            if (OnVREvent_OverlayShown != null)
                OnVREvent_OverlayShown.Invoke(Data);
        }

        public void RaiseOnVREvent_Quit(VREvent_Data_t Data)
        {
            if (OnVREvent_Quit != null)
                OnVREvent_Quit.Invoke(Data);
        }
        #endregion

        public abstract void Start();
        public abstract void Refresh();
    }
}
