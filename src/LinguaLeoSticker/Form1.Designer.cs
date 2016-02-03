namespace LinguaLeoSticker
{
    partial class frmSticker
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lb_text = new System.Windows.Forms.Label();
            this.lb_text_translate = new System.Windows.Forms.Label();
            this.tmrChangeWord = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lb_text
            // 
            this.lb_text.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lb_text.AutoSize = true;
            this.lb_text.Location = new System.Drawing.Point(76, 35);
            this.lb_text.Name = "lb_text";
            this.lb_text.Size = new System.Drawing.Size(96, 13);
            this.lb_text.TabIndex = 0;
            this.lb_text.Text = "Lingua Leo Sticker";
            this.lb_text.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_text_MouseDown);
            this.lb_text.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_text_MouseMove);
            this.lb_text.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_text_MouseUp);
            // 
            // lb_text_translate
            // 
            this.lb_text_translate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lb_text_translate.AutoSize = true;
            this.lb_text_translate.Location = new System.Drawing.Point(76, 133);
            this.lb_text_translate.Name = "lb_text_translate";
            this.lb_text_translate.Size = new System.Drawing.Size(90, 13);
            this.lb_text_translate.TabIndex = 1;
            this.lb_text_translate.Text = "LinguaLeoSticker";
            this.lb_text_translate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseDown);
            this.lb_text_translate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseMove);
            this.lb_text_translate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseUp);
            // 
            // tmrChangeWord
            // 
            this.tmrChangeWord.Interval = 1000;
            this.tmrChangeWord.Tick += new System.EventHandler(this.tmrChangeWord_Tick);
            // 
            // frmSticker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 222);
            this.Controls.Add(this.lb_text_translate);
            this.Controls.Add(this.lb_text);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSticker";
            this.ShowInTaskbar = false;
            this.Text = "LinguaLeoSticker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSticker_FormClosing);
            this.Load += new System.EventHandler(this.frmSticker_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_text;
        private System.Windows.Forms.Label lb_text_translate;
        private System.Windows.Forms.Timer tmrChangeWord;
    }
}

