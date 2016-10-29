using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

using ConfigFile;
using KeyboardHook;
using DictonaryManager;

namespace LinguaLeoSticker
{
    public partial class frmSticker : Form
    {
        private Config AppConf = null;

        private Point mouseOffset;
        private bool isMouseDown = false;

        private int TimerFirstWord = 1000;
        private int TimerSecondWord = 1000;

        private int step_display = 0;
        private string[] current_couple = null;

        private bool Form_Is_Extended = false;

        const int border_size = 10;

        const int ButtonNaviWidth = 50;

        private DictMng Dict = new DictMng();

        private bool isDoubleEnterInTxtWord = false;

        static Keys PrevKey;


        GlobalKeyboardHook hk = new GlobalKeyboardHook();



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
            ///try open dictonary
            if(Dict.Open(Path.Combine(Application.StartupPath,AppConf.DictonaryPath)))
            {
                //lb_translate.Text = DictonatyPath;
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
            if ((key == Keys.F) && (PrevKey == Keys.LMenu))
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
                current_couple = Dict.GetNextCouple();
                
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
                string[] couple = Dict.GetCurrentCouple();
                if (couple != null)
                {
                    DisplayCouple(couple[0], couple[1]);
                }

                txtWord.Text = "";
                txtTranslate.Text = "";
                txtWord.Select();
                txtWord.Focus();

                if (Top < 0)
                {
                    Top = 0;
                }

                int ScreenWidth = Screen.FromControl(this).Bounds.Width;
                if (Left > ScreenWidth - Width)
                {
                    Left = ScreenWidth - Width;
                }
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
            if (!Dict.AddWord(txtWord.Text, txtTranslate.Text))
            {
                MessageBox.Show("Cant add word to dictonary");
            }
            else
            {
                Form_Is_Extended = false;
                StartShow();
                AlignTextOnForm();
                Dict.Save(Path.Combine(Application.StartupPath, AppConf.DictonaryPath));
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
            string[] couple = Dict.GetNextCouple();
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            string[] couple = Dict.GetPrevCouple() ;
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string[] couple = Dict.GetCurrentCouple();
            if (couple != null)
            {
                Dict.RemoveWord(couple);
                Dict.Save(Path.Combine(Application.StartupPath, AppConf.DictonaryPath));
            }

            couple = Dict.GetCurrentCouple();
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

            if (e.KeyChar == (char) Keys.Enter)
            {
                if (isDoubleEnterInTxtWord)
                {
                    AddToDictonary();
                    return;
                }

                isDoubleEnterInTxtWord = true;
            }
            else
            {
                isDoubleEnterInTxtWord = false;
            }

            Keys crt_v = Keys.Capital | Keys.RButton;

            if ((e.KeyChar == (char)Keys.Enter) || (e.KeyChar == (char)crt_v))
            {
                if (txtWord.Text != "")
                {
                    LinguaLeoAPI llApi = new LinguaLeoAPI();
                    txtTranslate.Text = llApi.GetTranslate(txtWord.Text);
                }
            }
        }
    }
}
