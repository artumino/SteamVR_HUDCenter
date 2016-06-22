using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using Valve.VR;
using SteamVR_HUDCenter.Elements;

namespace SteamVR_HUDCenter
{
    public class HUDCenterController
    {
        public SteamVR VR { get; private set; }
        public bool _IsRunning { get; private set;}

        private List<Handlable> RegisteredItems = new List<Handlable>(1);
        private List<uint> Notifications = new List<uint>(1);

        [STAThread]
        public void Init(EVRApplicationType ApplicationType)
        {
            //Dummy OpenTK Window
            GameWindow window = new GameWindow(300, 300);
            VR = SteamVR.instance;

            EVRInitError error = EVRInitError.None;
            OpenVR.Init(ref error, ApplicationType);

            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing OpenVR!");

            OpenVR.GetGenericInterface(OpenVR.IVRCompositor_Version, ref error);
            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing Compositor!");

            OpenVR.GetGenericInterface(OpenVR.IVROverlay_Version, ref error);
            if (error != EVRInitError.None)
                throw new Exception("An error occured while initializing Overlay!");

            System.Threading.Thread OverlayThread = new System.Threading.Thread(new System.Threading.ThreadStart(OverlayCycle));
            OverlayThread.IsBackground = true;
            OverlayThread.Start();
        }

        public void Init()
        {
            Init(EVRApplicationType.VRApplication_Other);
        }

        public void Stop()
        {
            if (_IsRunning)
            {
                //Cleans overlays
                foreach (Overlay overlay in GetRegisteredOverlays())
                {
                    OpenVR.Overlay.ClearOverlayTexture(overlay.Handle);
                    OpenVR.Overlay.DestroyOverlay(overlay.Handle);
                }
                RegisteredItems.Clear();
                ClearNotifications();
            }
            _IsRunning = false;
        }

        #region HandlableManagement
        //Register new item to our list, this method is purely used to avoid duplicates in our list
        public void RegisterNewItem(Handlable item)
        {
            if (RegisteredItems.Contains<Handlable>(item))
                throw new Exception("Item with the same name and type already registered.");
            AssingHandle(item);
            RegisteredItems.Add(item);
            item.Init(this);
        }

        public IEnumerable<Overlay> GetRegisteredOverlays()
        {
            return RegisteredItems.Where(hand => hand is Overlay).Select<Handlable, Overlay>(hand => (Overlay)hand);
        }

        //Double checks internal handles to reduce Steam work
        private void AssingHandle(Handlable item)
        {
            Random rnd = new Random();
            do
            {
                item.Handle = (ulong)rnd.Next();
                foreach (Handlable hd in RegisteredItems)
                    if (hd.Handle == item.Handle)
                    {
                        item.Handle = 0;
                        break;
                    }
            } while (item.Handle == 0);
        }

        private uint GetNewNotificationID()
        {
            Random rnd = new Random();
            uint ID = 0;
            do
            {
                ID = (uint)rnd.Next();
                foreach (uint nID in Notifications)
                    if (nID == ID)
                    {
                        ID = 0;
                        break;
                    }
            } while (ID == 0);
            return ID;
        }
        #endregion

        #region Notifications
        public uint DisplayNotification(string Message, Overlay Overlay, EVRNotificationType Type, EVRNotificationStyle Style, NotificationBitmap_t Bitmap)
        {
            uint ID = GetNewNotificationID();
            OpenVR.Notifications.CreateNotification(Overlay.Handle, 0, Type, Message, Style, ref Bitmap, ref ID);
            Notifications.Add(ID);
            return ID;
        }

        public void CloseNotification(uint ID)
        {
            OpenVR.Notifications.RemoveNotification(ID);
            Notifications.Remove(ID);
        }

        public void ClearNotifications()
        {
            foreach (uint ID in Notifications)
                OpenVR.Notifications.RemoveNotification(ID);
            Notifications.Clear();
        }
        #endregion

        #region AppCycle
        public void OverlayCycle()
        {
            _IsRunning = true;
            while (_IsRunning)
            {
                foreach (Overlay overlay in GetRegisteredOverlays())
                    if (overlay.InputMethod == VROverlayInputMethod.Mouse)
                        HandleVRInput(overlay);
                System.Threading.Thread.Sleep(20);
            }
        }

        public void HandleVRInput(Overlay Overlay)
        {
            for (uint unDeviceId = 1; unDeviceId < OpenVR.k_unControllerStateAxisCount; unDeviceId++)
            {
                if (OpenVR.Overlay.HandleControllerOverlayInteractionAsMouse(Overlay.Handle, unDeviceId))
                {
                    break;
                }
            }

            VREvent_t vrEvent = new VREvent_t();
            while (OpenVR.Overlay.PollNextOverlayEvent(Overlay.Handle, ref vrEvent, (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VREvent_t))))
            {
                switch (vrEvent.eventType)
                {
                    case (int)EVREventType.VREvent_MouseMove:
                        Overlay.RaiseOnVREvent_MouseMove(vrEvent.data);
                        break;
                    case (int)EVREventType.VREvent_MouseButtonDown:
                        Overlay.RaiseOnVREvent_MouseButtonDown(vrEvent.data);
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
        #endregion
    }
}
