using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConfigFile;

namespace LinduaLeoSticker
{
    public partial class frmSticker : Form
    {
        private Point mouseOffset;
        private bool isMouseDown = false;

        private int TimerFirstWord = 0;
        private int TimerSecondWord = 0;
        private string DictonatyPath = "";

        public frmSticker()
        {
            TopMost = true;
            InitializeComponent();
            Config AppConf = new Config("config.xml");

            this.Height = AppConf.Height;
            this.Width = AppConf.Width;
            this.Top = AppConf.Y;
            this.Left = AppConf.X;
            this.BackColor = AppConf.BackgroundColor;
            this.lb_text.ForeColor = AppConf.TextColor;
            this.lb_text_translate.ForeColor = AppConf.TextTranslateColor;
            this.lb_text.Font = AppConf.TextFont;
            this.lb_text_translate.Font = AppConf.TextTranslateFont;
            this.TimerFirstWord = AppConf.TimeText;
            this.TimerSecondWord = AppConf.TimeTextTranslate;
            this.DictonatyPath = AppConf.DictonaryPath;


        
        }

        private void frmSticker_MouseUp(object sender, MouseEventArgs e)
        {
            // Changes the isMouseDown field so that the form does
            // not move unless the user is pressing the left mouse button.
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void frmSticker_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void frmSticker_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        private void frmSticker_Load(object sender, EventArgs e)
        {
            ///try open dictonery
            if (DictonatyPath != "")
            {

            }
            else
            {
                lb_text_translate.Text = "Can\'t open dictonary file!";
            }

            AlignTextOnForm();

        }

        private void AlignTextOnForm()
        {
            this.lb_text.Top = 10;
            this.lb_text_translate.Top = this.lb_text.Height + this.lb_text.Top + 10;


            int freespace_first_word = this.Width - this.lb_text.Width;
            if (freespace_first_word > 0)
            {
                this.lb_text.Left = freespace_first_word / 2;
            }
            else
            {
                this.lb_text.Left = 0;
            }
           


            int freespace_second_word = this.Width - this.lb_text_translate.Width;
            if (freespace_first_word > 0)
            {
                this.lb_text_translate.Left = freespace_second_word / 2;
            }
            else
            {
                this.lb_text_translate.Left = 0;
            }




        }
    }
}
