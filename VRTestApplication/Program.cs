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
using SteamVR_HUDCenter.Elements.Forms;

namespace VRTestApplication
{
    class Program
    {
        public static HUDCenterController VRController;

        [STAThread]
        static void Main(string[] args)
        {
            VRController = new HUDCenterController();
            VRController.Init();

            FormOverlay dashOverlay = new FormOverlay("Test", @"Resources/white-lambda.png", 2.0f, new TestForm());
            MainOverlay handOverlay = new MainOverlay("handOverlay", 1.0f, dashOverlay);
            VRController.RegisterNewItem(dashOverlay);
            VRController.RegisterNewItem(handOverlay);
            handOverlay.SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole.LeftHand);
            System.Windows.Forms.Application.Run();
            Console.ReadLine();
            VRController.Stop();
        }
    }
}
