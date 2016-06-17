using System;
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
    public class VRForm : Form
    {
        public delegate void PaintEvent(Control control, Point delta, PaintEventArgs e);
        public event PaintEvent OnVRPaint;

        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEBUTTONDOWN = 0x0207;
        public const int WM_MOUSEBUTTONUP = 0x0208;

        //Mouse Buttons
        public const int MK_LBUTTON = 0x0001;
        public const int MK_RBUTTON = 0x0002;
        public const int MK_MBUTTON = 0x0010;

        public VRForm() : base()
        {
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (Control control in Controls)
                control.Paint += Control_Paint;
        }

        public void SimulateMouseMoveEvent(int x, int y, int delta)
        {
            Invoke(new Action(() => {
                Control Affected = GetChildAtPoint(new Point(x, y));
                if (Affected != null)
                {
                    IntPtr lParam = (IntPtr)((Int32)((Int16)y << 16) + (Int16)x);
                    Message fakeMessage = Message.Create(Affected.Handle, WM_MOUSEMOVE, (IntPtr)0x0000, lParam);
                    this.WndProc(ref fakeMessage);
                }
            }));
            //this.RaiseMouseEvent("MouseMove", new MouseEventArgs(MouseButtons.None, 0, x, y, delta));
            //OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, x, y, delta));
        }

        private int ParseButton(MouseButtons button)
        {
            switch(button)
            {
                case MouseButtons.Right:
                    return MK_RBUTTON;
                case MouseButtons.Left:
                    return MK_LBUTTON;
                case MouseButtons.Middle:
                    return MK_MBUTTON;
                default:
                    return 0x0000;
            }
        }

        public void SimulateMouseDownEvent(MouseButtons button, int x, int y, int delta)
        {
            Invoke(new Action(() =>
            {
                Point ScreenP = PointToScreen(new Point(x, y));
                Control Affected = GetChildAtPoint(new Point(x,y));
                if (Affected != null)
                {
                    IntPtr lParam = (IntPtr)((Int32)((Int16)ScreenP.Y << 16) + (Int16)ScreenP.X);
                    Message fakeMessage = Message.Create(Affected.Handle, WM_MOUSEBUTTONDOWN, (IntPtr)ParseButton(button), lParam);
                    this.WndProc(ref fakeMessage);
                }
            }));
            //this.RaiseMouseEvent("MouseDown", new MouseEventArgs(button, 0, x, y, delta));
            //OnMouseDown(new MouseEventArgs(button, 0, x, y, delta));
        }

        public void SimulateMouseUpEvent(MouseButtons button, int x, int y, int delta)
        {
            Invoke(new Action(() =>
            {
                Point ScreenP = PointToScreen(new Point(x, y));
                Control Affected = GetChildAtPoint(new Point(x, y));
                if (Affected != null)
                {
                    IntPtr lParam = (IntPtr)((Int32)((Int16)ScreenP.Y << 16) + (Int16)ScreenP.X);
                    Message fakeMessage = Message.Create(Affected.Handle, WM_MOUSEBUTTONUP, (IntPtr)ParseButton(button), lParam);
                    this.WndProc(ref fakeMessage);
                }
            }));
            //this.RaiseMouseEvent("MouseUp", new MouseEventArgs(button, 0, x, y, delta));
            //OnMouseUp(new MouseEventArgs(button, 0, x, y, delta));
        }
        
        /*protected override void OnPaint(PaintEventArgs e)
        {
            // Call the OnPaint method of the base class.
            base.OnPaint(e);

            //Notify item changed
            if (OnVRPaint != null)
                OnVRPaint.Invoke(this, Point.Empty, e);
        }*/

        private void Control_Paint(object sender, PaintEventArgs e)
        {
            Point delta = Point.Empty;
            if (sender is Control)
            {
                delta = new Point(((Control)sender).Left, ((Control)sender).Top);

                //Notify item changed
                if (OnVRPaint != null)
                    OnVRPaint.Invoke((Control)sender, delta, e);
            }
        }
    }
}
