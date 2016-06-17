﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRTestApplication
{
    public partial class TestForm : VRTestApplication.VRForm
    {
        public TestForm()
        {
            InitializeComponent();
            MouseDown += TestForm_MouseDown;
            MouseUp += TestForm_MouseUp;
        }

        private void TestForm_MouseUp(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("MouseUp " + e.X + "," + e.Y);
        }

        private void TestForm_MouseDown(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("MouseDown " + e.X + "," + e.Y);
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseClick " + e.X + "," + e.Y);
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseEnter");
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseDown " + e.X + "," + e.Y);
        }
    }
}
