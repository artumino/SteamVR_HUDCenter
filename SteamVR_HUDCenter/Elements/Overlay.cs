using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Valve.VR;
using OpenTK;

namespace SteamVR_HUDCenter.Elements
{
    public class Overlay : Handlable
    {
        public VROverlayInputMethod InputMethod { get; private set; }
        public float Width { get; private set; }
        public bool IsDashboardWidget { get; private set; }
        public FileInfo ThumbnailPath { get; private set; }
        public Handlable Thumbnail { get; private set; }
        public bool IsVisible { get; private set; }

        protected HmdMatrix34_t identity = OTK_Utils.OpenTKMatrixToOpenVRMatrix(new Matrix3x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0)
        ));

        public Overlay(string FriendlyName,
            float Width,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None)
            : base(FriendlyName) //Registers Item Handle
        {
            this.Width = Width;
            this.IsDashboardWidget = false;
            this.ThumbnailPath = null;
            this.InputMethod = InputMethod;
        }

        public Overlay(string FriendlyName,
            string ThumbnailPath,
            float Width,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None) 
            : base(FriendlyName) //Registers Item Handle
        {
            this.Width = Width;
            this.IsDashboardWidget = true;
            this.ThumbnailPath = new FileInfo(ThumbnailPath);
            this.InputMethod = InputMethod;
        }

        public override void Init(HUDCenterController Controller)
        {
            base.Init(Controller);

            EVROverlayError overlayError;
            if (this.IsDashboardWidget)
            {
                //If this is a Dashboard Widget, Create a Thumbnail item
                Thumbnail = new Handlable(this.Key + this.Name + "_Thumbnail");

                if (this.ThumbnailPath.Exists)
                    OpenVR.Overlay.SetOverlayFromFile(Thumbnail.Handle, ThumbnailPath.FullName);
                else
                    OpenVR.Overlay.SetOverlayFromFile(Thumbnail.Handle, null);

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
            if(this.Controller != null)
                OpenVR.Overlay.SetOverlayWidthInMeters(this.Handle, Width);
        }

        public void SetOverlayInputMethod(VROverlayInputMethod InputMethod)
        {
            this.InputMethod = InputMethod;
            if (this.Controller != null)
                OpenVR.Overlay.SetOverlayInputMethod(this.Handle, InputMethod);
        }

        public void SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole Device)
        {
            SetOverlayTransformTrackedDeviceRelative(Device, identity);
        }

        public void SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole Device, HmdMatrix34_t matrix)
        {
            if (this.Controller != null)
                OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(this.Handle, OpenVR.System.GetTrackedDeviceIndexForControllerRole(Device), ref matrix);
        }

        public void SetOverlayTransformTrackedDeviceRelative(uint DeviceIndex)
        {
            SetOverlayTransformTrackedDeviceRelative(DeviceIndex, identity);
        }

        public void SetOverlayTransformTrackedDeviceRelative(uint DeviceIndex, HmdMatrix34_t matrix)
        {
            if (this.Controller != null)
                OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(this.Handle, DeviceIndex, ref matrix);
        }

        public void Show()
        {
            this.IsVisible = true;
            if (this.Controller != null)
                OpenVR.Overlay.ShowOverlay(this.Handle);
        }

        public void Hide()
        {
            this.IsVisible = false;
            if (this.Controller != null)
                OpenVR.Overlay.HideOverlay(this.Handle);
        }

        public void ToggleVisibility()
        {

            if (this.Controller != null)
            {
                if (this.IsVisible)
                    OpenVR.Overlay.HideOverlay(this.Handle);
                else
                    OpenVR.Overlay.ShowOverlay(this.Handle);
            }
            this.IsVisible = !this.IsVisible;
        }
        #endregion

        #region Events
        public virtual void OnVREvent_MouseMove(VREvent_Data_t Data) { }
        public virtual void OnVREvent_MouseButtonDown(VREvent_Data_t Data) { }
        public virtual void OnVREvent_MouseButtonUp(VREvent_Data_t Data) { }
        public virtual void OnVREvent_OverlayShown(VREvent_Data_t Data) { }
        public virtual void OnVREvent_Quit(VREvent_Data_t Data) { }
        public virtual void OnVREvent_ButtonPress(VREvent_Data_t Data) { }
        public virtual void OnVREvent_ButtonTouch(VREvent_Data_t Data) { }
        public virtual void OnVREvent_ButtonUnpress(VREvent_Data_t Data) { }
        public virtual void OnVREvent_ButtonUntouch(VREvent_Data_t Data) { }
        public virtual void OnVREvent_TouchPadMove(VREvent_Data_t Data) { }
        public virtual void OnVREvent_Scroll(VREvent_Data_t Data) { }
        #endregion

        public virtual void Start() { }
        public virtual void Refresh() { }
    }
}
