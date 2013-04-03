using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using Microsoft.VisualBasic;

namespace VirtualBeach
{
    /// <summary>
    /// define an application start-up screen and maybe close it 
    /// after a specified number of seconds (see the contstructor)
    /// </summary>
    public partial class frmVBSplashScreen : Form
    {
        //volatile strings - change as needed
        private string _contact = @"mailto:cyterski.mike@epa.gov";
        private string _website = @"http://www.epa.gov/ceampubl/swater/vb2/index.html";

        private System.Timers.Timer _timer = null;
        private frmVBSplashScreen _me = null;
        private bool _IsTimed = false;
        private int _duration = -1;

        /// <summary>
        /// constructor for the splash screen takes 2 args
        /// </summary>
        /// <param name="IsTimed">set true if displayed for a set time</param>
        /// <param name="duration">int seconds for a timed display; ignored if not timed</param>
        public frmVBSplashScreen(bool IsTimed, int duration)
        {
            InitializeComponent();
            _IsTimed = IsTimed;
            // display time in seconds; timer uses miliseconds
            _duration = duration * 1000; 
            //save this off to close it from the timer thread
            _me = this;

            //get the project's assemly file version for display

            var pos = this.PointToScreen(label3.Location);
            pos = pictureBox1.PointToClient(pos);
            label3.Parent = pictureBox1;
            label3.Location = pos;
            label3.BackColor = Color.Transparent;


            label3.Text = "Version " + Application.ProductVersion.Substring(0, 3);
        }

        /// <summary>
        /// if timed display, start a timer and subscribe to the _duration elapsedtimer event
        /// otherwise just load the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmVBSplashScreen_Load(object sender, EventArgs e)
        {
            
            

            //display for a set time...
            if (_IsTimed)
            {
                _timer = new System.Timers.Timer(_duration);
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(CloseSplash);
                _timer.Enabled = true;
            }
            //... or not
        }

        /// <summary>
        /// time has expired; close this down
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void CloseSplash(object o, System.Timers.ElapsedEventArgs e)
        {
            _timer.Dispose();
            //Close()
            //wrong thread (timer thread), try this...
            //if (  _me.Handle != null  ) 
                this.Invoke((MethodInvoker)delegate { _me.Close(); }); 
        }

        /// <summary>
        /// open a browser and visit the website - change the global _website for some other place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //string site = @"http://www.epa.gov/ceampubl/swater/vb2/index.html";
            System.Diagnostics.Process.Start(_website);
        }

        /// <summary>
        /// send an email to the contact - change the global _contact for someone else
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //string contact = @"mailto:cyterski.mike@epa.gov";
            System.Diagnostics.Process.Start(_contact);
        }

        private void frmVBSplashScreen_MouseClick(object sender, MouseEventArgs e)
        {
            //if (_timer != null) _timer.Dispose();
            //this.Close();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (_timer != null) _timer.Dispose();
            //this.Close();
        }
    }
}
