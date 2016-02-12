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
using System.IO;
using Microsoft.Win32;

namespace LinguaLeoSticker
{
    public partial class frmSticker : Form
    {
        Config AppConf = null;

        private Point mouseOffset;
        private bool isMouseDown = false;

        private int TimerFirstWord = 1000;
        private int TimerSecondWord = 1000;
        private string DictonatyPath = "";

        private string[] LocalDictonary = null;
        private int DictonarySeqPos = 0;

        private int step_display = 0;
        private string[] current_couple = null;

        public frmSticker()
        {
            TopMost = true;
            InitializeComponent();
            AppConf = new Config("config.xml");

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

            RegistryKey rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (AppConf.AutoLoad)
            {
                rkey.SetValue(Application.ProductName, Application.ExecutablePath.ToString());
            }
            else
            {
                try
                {
                    rkey.DeleteValue(Application.ProductName);
                }
                catch(Exception)
                {

                }
            }
            
        }

        private void MoveForm_DownEvent(MouseEventArgs e)
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

        private void MoveForm_MoveEvent()
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void MoveForm_UpEvent(MouseEventArgs e)
        {
            // Changes the isMouseDown field so that the form does
            // not move unless the user is pressing the left mouse button.
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;

                AppConf.Y = this.Top;
                AppConf.X = this.Left;
            }
        }

        private void lb_text_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm_DownEvent(e);
        }

        private void lb_text_MouseUp(object sender, MouseEventArgs e)
        {
            MoveForm_UpEvent(e);
        }

        private void lb_text_MouseMove(object sender, MouseEventArgs e)
        {
            MoveForm_MoveEvent();
        }

        private void lb_text_translate_MouseUp(object sender, MouseEventArgs e)
        {
            MoveForm_UpEvent(e);
        }

        private void lb_text_translate_MouseMove(object sender, MouseEventArgs e)
        {
            MoveForm_MoveEvent();
        }

        private void lb_text_translate_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm_DownEvent(e);
        }

        private void frmSticker_MouseUp(object sender, MouseEventArgs e)
        {
            MoveForm_UpEvent(e);
        }

        private void frmSticker_MouseMove(object sender, MouseEventArgs e)
        {
            MoveForm_MoveEvent();
        }

        private void frmSticker_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm_DownEvent(e);
        }

        private void frmSticker_Load(object sender, EventArgs e)
        {
            ///try open dictonery
            if (DictonaryLoad(DictonatyPath) == true)
            {
                lb_text_translate.Text = DictonatyPath;
                StartShow();
            }
            else
            {
                lb_text_translate.Text = "Can\'t open dictonary file!";
            }


            this.Top = AppConf.Y;
            this.Left = AppConf.X;
               
            AlignTextOnForm();
        }

        private void AlignTextOnForm()
        {
            const int border_size = 10;


            this.Height = this.lb_text.Height + this.lb_text_translate.Height + 3 * border_size;

            this.lb_text.Top = border_size;
            this.lb_text_translate.Top = this.lb_text.Height + this.lb_text.Top + border_size;


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

        private bool DictonaryLoad(string path)
        {
            bool Ret = false;

            if (path != "")
            {               
                LocalDictonary = File.ReadAllLines(path,Encoding.GetEncoding(1251));
                DictonarySeqPos = 0;
    
                Ret = true;
            }

            return Ret;
        }

        private string[] DictonaryGetNextCouple(bool random)
        {
            string[] couple = new string[2];
            const char separator = ':';

            if (random)
            {
                DictonarySeqPos = new Random().Next(0, LocalDictonary.Count());
            }
            else
            {
                ++DictonarySeqPos;
                if (DictonarySeqPos >= LocalDictonary.Count())
                {
                    DictonarySeqPos = 0;
                }
            }

            string Line = LocalDictonary[DictonarySeqPos];


            int pos = Line.IndexOf(separator);
            if (pos != -1)
            {

                couple[0] = Line.Substring(0, pos);
                couple[1] = Line.Substring(pos + 1);

            }

            return couple;
        }

        private void tmrChangeWord_Tick(object sender, EventArgs e)
        {
            if ((step_display++ & 1) == 0)
            {
                current_couple = DictonaryGetNextCouple(false);
                
                if (current_couple != null)
                {
                    this.lb_text.Text = current_couple[0];
                    this.lb_text_translate.Text = "";
                    tmrChangeWord.Interval = TimerFirstWord;
                }
            }
            else
            {
                if (current_couple != null)
                {
                    this.lb_text_translate.Text = current_couple[1];
                    tmrChangeWord.Interval = TimerSecondWord;
                }
            }

            AlignTextOnForm();            
        }

        void StartShow()
        {
            tmrChangeWord.Interval = 1;
            tmrChangeWord.Enabled = true;
        }

        private void frmSticker_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppConf.saveConfig();   
        }

    }
}
