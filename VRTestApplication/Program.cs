﻿using System;
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

            FormOverlay dashOverlay = new FormOverlay("Test", @"Resources\white-lambda.png", 2.0f, new TestForm());
            MainOverlay handOverlay = new MainOverlay("handOverlay", 0.5f, dashOverlay);
            VRController.RegisterNewItem(dashOverlay);
            VRController.RegisterNewItem(handOverlay);
            handOverlay.SetOverlayTransformTrackedDeviceRelative(ETrackedControllerRole.LeftHand, OTK_Utils.OpenTKMatrixToOpenVRMatrix(new Matrix3x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, -1, 0, 0.1f),
                new Vector4(0, 0, 1, 0)
            )));
            handOverlay.Show();
            System.Windows.Forms.Application.Run();
            Console.ReadLine();
            VRController.Stop();
        }
    }
}
