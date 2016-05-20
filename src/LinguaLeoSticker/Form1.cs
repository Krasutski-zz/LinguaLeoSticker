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

using KeyboardHook;

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

        private bool Form_Is_Extended = false;

        const char separator = ':';
        const int border_size = 10;

        const int ButtonNaviWidth = 50;


        static Keys PrevKey;


        GlobalKeyboardHook hk = new GlobalKeyboardHook();

        enum DictonatyAccess_e
        {
            DictAccess_Next,
            DictAccess_Prev,
            DictAccess_Current,
            DictAccess_Random
        };


        public frmSticker()
        {
            TopMost = true;
            InitializeComponent();

            AppConf = new Config(Path.Combine(Application.StartupPath,"config.xml"));

            this.Height = AppConf.Height;
            this.Width = AppConf.Width;
            this.Top = AppConf.Y;
            this.Left = AppConf.X;
            this.BackColor = AppConf.BackgroundColor;
            lb_word.ForeColor = AppConf.TextColor;
            lb_translate.ForeColor = AppConf.TextTranslateColor;
            lb_word.Font = AppConf.TextFont;
            lb_translate.Font = AppConf.TextTranslateFont;
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(),"Error");
                }
            }
        }

        private bool TextBox_CommonKey_Press(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.F16)
            {
                ((TextBox)sender).Text = "";
                e.Handled = true;
            }
            else
            {
                return false;
            }

            return true;
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
            if (DictonaryLoad(Path.Combine(Application.StartupPath,DictonatyPath)) == true)
            {
                lb_translate.Text = DictonatyPath;
                StartShow();
            }
            else
            {
                lb_translate.Text = "Can\'t open dictonary file!";
            }

            this.Top = AppConf.Y;
            this.Left = AppConf.X;
               
            AlignTextOnForm();

            /* Add align text boxses */
            txtWord.Left = 0;
            txtWord.Top = lb_translate.Top + lb_translate.Height + border_size;
            txtWord.Width = this.Width;

            txtTranslate.Left = 0;
            txtTranslate.Top = txtWord.Top + txtWord.Height;
            txtTranslate.Width = this.Width;

            /* Add align button */
            btnAddToDictonary.Left = 0;
            btnAddToDictonary.Top = txtTranslate.Top + txtTranslate.Height;
            btnAddToDictonary.Width = this.Width;

            btn_Back.Top = btn_Delete.Top = btn_Forward.Top = btnAddToDictonary.Top + btnAddToDictonary.Height;
            btn_Back.Width = btn_Forward.Width = ButtonNaviWidth;

            btn_Back.Left = 0;
            btn_Back.Width = ButtonNaviWidth;

            btn_Forward.Left = this.Width - ButtonNaviWidth;
            btn_Forward.Width = ButtonNaviWidth;

            btn_Delete.Left = btn_Back.Width;
            btn_Delete.Width = this.Width - 2 * ButtonNaviWidth;

            hk.key_hook_evt += key_hook;
        }

        public void key_hook(Keys key)
        {
            if ((key == Keys.X) && (PrevKey == Keys.LMenu))
            {
                ExtendColapseForm();
            }
            else if (key == Keys.Escape && Form_Is_Extended)
            {
                ExtendColapseForm();
            }

            PrevKey = key;
        }

        private int GetFormHeight(bool extended)
        {            
            if (extended)
            {
                return lb_word.Height + 
                    lb_translate.Height + 
                    3 * border_size + 
                    txtTranslate.Height + 
                    txtWord.Height + 
                    btnAddToDictonary.Height + 
                    btn_Delete.Height;
            }

            return lb_word.Height + lb_translate.Height + 3 * border_size;
        }

        private void AlignTextOnForm()
        {

            this.Height = GetFormHeight(Form_Is_Extended);

            lb_word.Top = border_size;
            lb_translate.Top = lb_word.Height + lb_word.Top + border_size;


            int freespace_first_word = this.Width - lb_word.Width;
            if (freespace_first_word > 0)
            {
                lb_word.Left = freespace_first_word / 2;
            }
            else
            {
                lb_word.Left = 0;
            }
           

            int freespace_second_word = this.Width - lb_translate.Width;
            if (freespace_first_word > 0)
            {
                lb_translate.Left = freespace_second_word / 2;
            }
            else
            {
                lb_translate.Left = 0;
            }
        }

        private bool DictonaryLoad(string path)
        {
            bool Ret = false;

            if (path != "")
            {
                LocalDictonary = File.ReadAllLines(path, System.Text.Encoding.Default/*Encoding.GetEncoding(1251)*/);
                DictonarySeqPos = 0;
    
                Ret = true;
            }

            return Ret;
        }

        private string[] DictonaryGetNextCouple(DictonatyAccess_e mode)
        {
            string[] couple = new string[2];

            if (LocalDictonary.Length == 0 || LocalDictonary == null)
            {
                return null;
            }

            if (mode == DictonatyAccess_e.DictAccess_Random)
            {
                DictonarySeqPos = new Random().Next(0, LocalDictonary.Count());
            }
            else if (mode == DictonatyAccess_e.DictAccess_Prev)
            {
                //Todo save random history and restore trace

                --DictonarySeqPos;
                if (DictonarySeqPos < 0)
                {
                    DictonarySeqPos = LocalDictonary.Count() - 1;
                }
            }
            else if (mode == DictonatyAccess_e.DictAccess_Next)
            {
                ++DictonarySeqPos;
                if (DictonarySeqPos >= LocalDictonary.Count())
                {
                    DictonarySeqPos = 0;
                }
            }

            string Line = "";

            try
            {
                Line = LocalDictonary[DictonarySeqPos];
            }
            catch (Exception e)
            {
                DictonarySeqPos = 0;
                Line = LocalDictonary[DictonarySeqPos];
            }


            int pos = Line.IndexOf(separator);
            if (pos != -1)
            {
                couple[0] = Line.Substring(0, pos);
                couple[1] = Line.Substring(pos + 1);
            }

            return couple;
        }

        private bool RemoveWordFromDictonary(string Item)
        {
            var list = new List<string>(LocalDictonary);
            list.Remove(Item);
            LocalDictonary = list.ToArray();

            return true;
        }

        private bool AddWordToDictonary(string Word, string Translate)
        {
            if (Word == "")
            {
                return false;
            }

            if (Translate == "")
            {
                return false;
            }

            string line = String.Format("{0}{1}{2}", Word, separator, Translate);

            if (LocalDictonary != null)
            {
                Array.Resize(ref LocalDictonary, LocalDictonary.Length + 1);
                LocalDictonary[LocalDictonary.Length - 1] = line;               
            }

            return true;
        }

        private bool SaveDictonary(string path)
        {
            if (LocalDictonary != null)
            {
                File.WriteAllLines(path, LocalDictonary, System.Text.Encoding.UTF8);
            }

            return true;
        }

        private void DisplayCouple(string Word, string Translate)
        {
            lb_word.Text = Word;
            lb_translate.Text = Translate;
            
            AlignTextOnForm();   
        }

        private void tmrChangeWord_Tick(object sender, EventArgs e)
        {
            if ((step_display++ & 1) == 0)
            {
                current_couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Next);
                
                if (current_couple != null)
                {
                    DisplayCouple(current_couple[0], "");
                    tmrChangeWord.Interval = TimerFirstWord;
                }
            }
            else
            {
                if (current_couple != null)
                {
                    DisplayCouple(current_couple[0], current_couple[1]);
                    tmrChangeWord.Interval = TimerSecondWord;
                }
            }         
        }

        void StartShow()
        {
            tmrChangeWord.Interval = 1;
            tmrChangeWord.Enabled = true;
        }

        void StopShow()
        {
            tmrChangeWord.Enabled = false;
        }

        private void frmSticker_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppConf.saveConfig();   
        }

        private void ExtendColapseForm()
        {
            Form_Is_Extended = !Form_Is_Extended;
            AlignTextOnForm();

            if (Form_Is_Extended)
            {
                StopShow();
                string[] couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Current);
                if (couple != null)
                {
                    DisplayCouple(couple[0], couple[1]);
                }

                txtWord.Text = "";
                txtTranslate.Text = "";
                txtWord.Focus();
            }
            else
            {
                StartShow();
            }
        }

        private void frmSticker_DoubleClick(object sender, EventArgs e)
        {
            ExtendColapseForm();
        }

        private void AddToDictonary()
        {
            if (!AddWordToDictonary(txtWord.Text, txtTranslate.Text))
            {
                MessageBox.Show("Cant add word to dictonary");
            }
            else
            {
                Form_Is_Extended = false;
                StartShow();
                AlignTextOnForm();
                SaveDictonary(Path.Combine(Application.StartupPath, DictonatyPath));
            }
        }

        private void btnAddToDictonary_Click(object sender, EventArgs e)
        {
            AddToDictonary();           
        }

        private void txtTranslate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!TextBox_CommonKey_Press(sender, e))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    AddToDictonary();
                }
            }
        }

        private void btn_Forward_Click(object sender, EventArgs e)
        {
            string[] couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Next);
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            string[] couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Prev);
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string[] couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Current);
            if (couple != null)
            {
                string item = string.Format("{0}{1}{2}", couple[0], separator, couple[1]);
                RemoveWordFromDictonary(item);
                SaveDictonary(Path.Combine(Application.StartupPath, DictonatyPath));
            }

            couple = DictonaryGetNextCouple(DictonatyAccess_e.DictAccess_Current);
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
            else
            {
                DisplayCouple("Dictonary", "Is empty");
            }
        }

        private void txtWord_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox_CommonKey_Press(sender, e);            
        }

    }
}
