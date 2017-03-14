using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using LinguaLeoSticker.Properties;
using Microsoft.Win32;

namespace LinguaLeoSticker
{
    public partial class FrmSticker : Form
    {
        private readonly Config _appConf;

        private Point _moveFormStartPoint;
        private bool _isMouseDown;

        private readonly int _timerSecondWord;

        private int _stepDisplay;

        private bool _formIsExtended;

        private const int BorderSize = 10;

        const int ButtonNaviWidth = 50;

        private readonly DictMng _dict = new DictMng();

        private bool _isDoubleEnterInTxtWord;

        private const Keys HotKey1 = Keys.LMenu;
        private const Keys HotKey2 = Keys.C;
        private bool _key1IsPressed;
        private bool _key2IsPressed;

        private readonly LinguaLeoApi _llApi = new LinguaLeoApi();

        private readonly GlobalKeyboardHook _hk = new GlobalKeyboardHook();

        public string[] CurrentCouple { get; set; }

        public FrmSticker()
        {
            TopMost = true;
            InitializeComponent();

            _appConf = new Config(Path.Combine(Application.StartupPath,"config.xml"));

            Height = _appConf.Height;
            Width = _appConf.Width;
            Top = _appConf.Y;
            Left = _appConf.X;
            BackColor = _appConf.BackgroundColor;
            lb_word.ForeColor = _appConf.TextColor;
            lb_translate.ForeColor = _appConf.TextTranslateColor;
            lb_word.Font = _appConf.TextFont;
            lb_translate.Font = _appConf.TextTranslateFont;
            TimerFirstWord2 = _appConf.TimeText;
            _timerSecondWord = _appConf.TimeTextTranslate;
           
            RegistryKey rkey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (_appConf.AutoLoad)
            {
                rkey?.SetValue(Application.ProductName, value: Application.ExecutablePath);
            }
            else
            {
                try
                {
                    rkey?.DeleteValue(Application.ProductName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString(),@"Error");
                }
            }
        }

        public sealed override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        public int TimerFirstWord => TimerFirstWord1;

        public int TimerFirstWord1 => TimerFirstWord2;

        public int TimerFirstWord2 { get; }

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


        private void MoveForm_DownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _moveFormStartPoint = new Point(Left - MousePosition.X, y: Top - MousePosition.Y);
                _isMouseDown = true;
            }
        }

        private void MoveForm_MoveEvent(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                Point mousePos = MousePosition;
                mousePos.Offset(_moveFormStartPoint.X, _moveFormStartPoint.Y);
                Location = mousePos;
            }
        }

        private void MoveForm_UpEvent(object sender, MouseEventArgs e)
        {
            // Changes the _isMouseDown field so that the form does
            // not move unless the user is pressing the left mouse button.
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = false;

                //save current position
                _appConf.Y = Top;
                _appConf.X = Left;
            }
        }       

        private void frmSticker_Load(object sender, EventArgs e)
        {
            //try open dictonary
            if (_dict.Open(Path.Combine(Application.StartupPath, _appConf.DictonaryPath)))
            {
                //lb_translate.Text = DictonatyPath;
                StartShow();
            }
            else
            {
                lb_translate.Text = Resources.FrmSticker_frmSticker_Load_Can_t_open_dictonary_file;

                try
                {
                    if (!_llApi.is_Auth())
                    {
                        _llApi.Auth(_appConf.LinguaLeoUser, _appConf.LinguaLeoPassword);
                    }

                    string[] llDict;
                    _llApi.GetUserDict(out llDict);

                    if (llDict.Length > 0)
                    {
                        _dict.Open(llDict);

                        //create dictonary file
                        _dict.Save(Path.Combine(Application.StartupPath, _appConf.DictonaryPath));
                        StartShow();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Top = _appConf.Y;
            Left = _appConf.X;

            AlignTextOnForm();

            /* Add align text boxses */
            txtWord.Left = 0;
            txtWord.Top = lb_translate.Top + lb_translate.Height + BorderSize;
            txtWord.Width = Width;

            txtTranslate.Left = 0;
            txtTranslate.Top = txtWord.Top + txtWord.Height;
            txtTranslate.Width = Width;

            /* Add align button */
            btnAddToDictonary.Left = 0;
            btnAddToDictonary.Top = txtTranslate.Top + txtTranslate.Height;
            btnAddToDictonary.Width = Width;

            btn_Back.Top = btn_Delete.Top = btn_Forward.Top = btnAddToDictonary.Top + btnAddToDictonary.Height;
            btn_Back.Width = btn_Forward.Width = ButtonNaviWidth;

            btn_Back.Left = 0;
            btn_Back.Width = ButtonNaviWidth;

            btn_Forward.Left = Width - ButtonNaviWidth;
            btn_Forward.Width = ButtonNaviWidth;

            btn_Delete.Left = btn_Back.Width;
            btn_Delete.Width = Width - 2 * ButtonNaviWidth;

            _hk.KeyHookEvt += key_hook;
        }

        public bool key_hook(int keybMsg, Keys key)
        {
            if (keybMsg <= 0) throw new ArgumentOutOfRangeException(nameof(keybMsg));
            GlobalKeyboardHook.KeyboardMessage km = (GlobalKeyboardHook.KeyboardMessage)keybMsg;

            if (key == HotKey1)
            {
                if (km == GlobalKeyboardHook.KeyboardMessage.WmKeydown || km == GlobalKeyboardHook.KeyboardMessage.WmSyskeydown)
                {
                    _key1IsPressed = true;
                }
                else if (km == GlobalKeyboardHook.KeyboardMessage.WmKeyup || km == GlobalKeyboardHook.KeyboardMessage.WmSyskeyup)
                {
                    _key1IsPressed = false;
                }
            }
            else if (key == HotKey2)
            {
                if (km == GlobalKeyboardHook.KeyboardMessage.WmKeydown || km == GlobalKeyboardHook.KeyboardMessage.WmSyskeydown)
                {
                    _key2IsPressed = true;
                }
                else if (km == GlobalKeyboardHook.KeyboardMessage.WmKeyup || km == GlobalKeyboardHook.KeyboardMessage.WmSyskeyup)
                {
                    _key2IsPressed = false;
                }
            }

            if ( _key1IsPressed && _key2IsPressed)
            {
                ExtendColapseForm();
                return true;
            }
            else if (km == GlobalKeyboardHook.KeyboardMessage.WmKeyup && key == Keys.Escape && _formIsExtended)
            {
                ExtendColapseForm();
            }

            return false;
        }

        private int GetFormHeight(bool extended)
        {            
            if (extended)
            {
                return lb_word.Height + 
                    lb_translate.Height + 
                    3 * BorderSize + 
                    txtTranslate.Height + 
                    txtWord.Height + 
                    btnAddToDictonary.Height + 
                    btn_Delete.Height;
            }

            return lb_word.Height + lb_translate.Height + 3 * BorderSize;
        }

        private void AlignTextOnForm()
        {

            Height = GetFormHeight(_formIsExtended);

            lb_word.Top = BorderSize;
            lb_translate.Top = lb_word.Height + lb_word.Top + BorderSize;


            var freespaceFirstWord = Width - lb_word.Width;
            if (freespaceFirstWord > 0)
            {
                lb_word.Left = freespaceFirstWord / 2;
            }
            else
            {
                lb_word.Left = 0;
            }


            var freespaceSecondWord = Width - lb_translate.Width;
            if (freespaceFirstWord > 0)
            {
                lb_translate.Left = freespaceSecondWord / 2;
            }
            else
            {
                lb_translate.Left = 0;
            }
        }


        private void DisplayCouple(string word, string translate)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));
            lb_word.Text = word;
            lb_translate.Text = translate;
            
            AlignTextOnForm();   
        }

        private void tmrChangeWord_Tick(object sender, EventArgs e)
        {
            if ((_stepDisplay++ & 1) == 0)
            {
                CurrentCouple = _appConf.RandomMode ? _dict.GetRandomCouple() : _dict.GetNextCouple();
                if (CurrentCouple != null)
                {
                    DisplayCouple(CurrentCouple[0], "");
                    tmrChangeWord.Interval = TimerFirstWord;
                }
            }
            else
            {
                if (CurrentCouple != null)
                {
                    DisplayCouple(CurrentCouple[0], CurrentCouple[1]);
                    tmrChangeWord.Interval = _timerSecondWord;
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
            _appConf.SaveConfig();   
        }

        private void ExtendColapseForm()
        {

            _formIsExtended = !_formIsExtended;
            AlignTextOnForm();

            int screenWidth = Screen.FromControl(this).Bounds.Width;
            if (_formIsExtended)
            {
                StopShow();
                string[] couple = _dict.GetCurrentCouple();
                if (couple != null)
                {
                    DisplayCouple(couple[0], couple[1]);
                }

                txtWord.Text = "";
                txtTranslate.Text = "";

                WindowState = FormWindowState.Minimized;
                WindowState = FormWindowState.Normal;

                ActiveControl = txtWord;
                txtWord.Select();
                txtWord.Focus();

                if (Top < 0)
                {
                    Top = 0;
                }

                if (Left <= screenWidth - Width) return;
                Left = screenWidth - Width;
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

            string tword = txtTranslate.Text.Trim();
            string word = txtWord.Text.Trim();

            if (!_llApi.is_Auth())
            {
                try
                {
                    _llApi.Auth(_appConf.LinguaLeoUser, _appConf.LinguaLeoPassword);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            if (_llApi.is_Auth())
            {
                _llApi.AddWord(word, tword, "");
            }

            if (!_dict.AddWord(word, tword))
            {
                MessageBox.Show(Resources.FrmSticker_frmSticker_Load_Can_t_open_dictonary_file);
            }
            else
            {
                _formIsExtended = false;
                StartShow();
                AlignTextOnForm();
                _dict.Save(Path.Combine(Application.StartupPath, _appConf.DictonaryPath));
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
            string[] couple = _dict.GetNextCouple();
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            string[] couple = _dict.GetPrevCouple() ;
            if (couple != null)
            {
                DisplayCouple(couple[0], couple[1]);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            string[] couple = _dict.GetCurrentCouple();
            if (couple != null)
            {
                _dict.RemoveWord(couple);
                _dict.Save(Path.Combine(Application.StartupPath, _appConf.DictonaryPath));
            }

            couple = _dict.GetCurrentCouple();
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
                if (_isDoubleEnterInTxtWord)
                {
                    _isDoubleEnterInTxtWord = false;
                    AddToDictonary();
                    return;
                }

                _isDoubleEnterInTxtWord = true;
            }
            else
            {
                _isDoubleEnterInTxtWord = false;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                if (txtWord.Text != "")
                {
                    txtTranslate.Text = _llApi.GetTranslate(txtWord.Text);
                }
            }
        }

        private void txtWord_KeyUp(object sender, KeyEventArgs e)
        {
            Keys crt_v = Keys.V | Keys.Control;

            if (e.KeyData == crt_v)
            {
                if (txtWord.Text != "")
                {
                    txtTranslate.Text = _llApi.GetTranslate(txtWord.Text);
                }
            }
        }
    }
}
