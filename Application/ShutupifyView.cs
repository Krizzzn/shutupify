using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shutupify;
using Shutupify.Settings;

namespace frm
{
    public partial class ShutupifyView : Form
    {
        AutoHooker sirHookalot;
        private Point _drag;

        public static string lastFoo;

        public ShutupifyView()
        {
            InitializeComponent();
            InitializeComponentCustom();

            FileReader settingsReader = null;
            if (System.IO.File.Exists("shutupify.settings"))
                settingsReader = new FileReader("shutupify.settings");
            sirHookalot = new AutoHooker(settingsReader);

            sirHookalot.ReactOnEvent += (t) => {
                try {
                    lastFoo = DateTime.Now.ToLongTimeString() + "::" + t.ToString();
                    RefreshBox(null, null);
                }
                catch (Exception e) {
                    lastFoo = e.Message;
                };
            };
            sirHookalot.Hookup();

            if (settingsReader == null)
                settingsReader.Save();
        }

        private void InitializeComponentCustom()
        {
            var appIconStream = System.Reflection.Assembly.GetExecutingAssembly().LoadEmbeddedAsStream("app-icon.ico");
            var icon = new System.Drawing.Icon(appIconStream);
            appIconStream.Dispose();

            SystemTrayIcon.Icon = icon;
            this.Icon = icon;

            Dragify(this);
            Whatsup.Text = " ";

            this.BackColor = Color.FromArgb(64, 62, 65);
            this.ForeColor = Color.FromArgb(184, 200, 43);
        }

        private void Dragify(Control c)
        {
            c.MouseDown += (e, v) => { _drag = v.Location; };
            c.MouseUp += (e, v) => { _drag = Point.Empty; };
            c.MouseMove += MoveWindowIfDragging;

            foreach (Control sub in c.Controls) {
                Dragify(sub);
            }
        }

        void MoveWindowIfDragging(object sender, MouseEventArgs e)
        {
            if (_drag == Point.Empty)
                return;

            this.Location = new Point(Cursor.Position.X - _drag.X, Cursor.Position.Y - _drag.Y);
        }

        private void RefreshBox(object sender, EventArgs e)
        {
            Whatsup.Text = lastFoo;
        }

        private void ShutupifyView_FormClosing(object sender, FormClosingEventArgs e)
        {
            sirHookalot.Clear();
            SystemTrayIcon.Visible = false;
            SystemTrayIcon.Dispose();
        }

        private void BringToFront(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.TopMost = true;
            this.TopMost = false;
        }

    }
}
