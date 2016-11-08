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
            this.lb_word = new System.Windows.Forms.Label();
            this.lb_translate = new System.Windows.Forms.Label();
            this.tmrChangeWord = new System.Windows.Forms.Timer(this.components);
            this.txtWord = new System.Windows.Forms.TextBox();
            this.txtTranslate = new System.Windows.Forms.TextBox();
            this.btnAddToDictonary = new System.Windows.Forms.Button();
            this.btn_Back = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_Forward = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_word
            // 
            this.lb_word.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lb_word.AutoSize = true;
            this.lb_word.Location = new System.Drawing.Point(76, 68);
            this.lb_word.Name = "lb_word";
            this.lb_word.Size = new System.Drawing.Size(96, 13);
            this.lb_word.TabIndex = 0;
            this.lb_word.Text = "Lingua Leo Sticker";
            this.lb_word.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_word_MouseDown);
            this.lb_word.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_word_MouseMove);
            this.lb_word.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_word_MouseUp);
            // 
            // lb_translate
            // 
            this.lb_translate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lb_translate.AutoSize = true;
            this.lb_translate.Location = new System.Drawing.Point(76, 166);
            this.lb_translate.Name = "lb_translate";
            this.lb_translate.Size = new System.Drawing.Size(90, 13);
            this.lb_translate.TabIndex = 1;
            this.lb_translate.Text = "LinguaLeoSticker";
            this.lb_translate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseDown);
            this.lb_translate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseMove);
            this.lb_translate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lb_text_translate_MouseUp);
            // 
            // tmrChangeWord
            // 
            this.tmrChangeWord.Interval = 1000;
            this.tmrChangeWord.Tick += new System.EventHandler(this.tmrChangeWord_Tick);
            // 
            // txtWord
            // 
            this.txtWord.Location = new System.Drawing.Point(12, 164);
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(100, 20);
            this.txtWord.TabIndex = 2;
            this.txtWord.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWord_KeyPress);
            this.txtWord.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtWord_KeyUp);
            // 
            // txtTranslate
            // 
            this.txtTranslate.Location = new System.Drawing.Point(12, 190);
            this.txtTranslate.Name = "txtTranslate";
            this.txtTranslate.Size = new System.Drawing.Size(100, 20);
            this.txtTranslate.TabIndex = 3;
            this.txtTranslate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTranslate_KeyPress);
            // 
            // btnAddToDictonary
            // 
            this.btnAddToDictonary.Location = new System.Drawing.Point(12, 216);
            this.btnAddToDictonary.Name = "btnAddToDictonary";
            this.btnAddToDictonary.Size = new System.Drawing.Size(75, 23);
            this.btnAddToDictonary.TabIndex = 4;
            this.btnAddToDictonary.Text = "Add";
            this.btnAddToDictonary.UseVisualStyleBackColor = true;
            this.btnAddToDictonary.Click += new System.EventHandler(this.btnAddToDictonary_Click);
            // 
            // btn_Back
            // 
            this.btn_Back.Location = new System.Drawing.Point(12, 245);
            this.btn_Back.Name = "btn_Back";
            this.btn_Back.Size = new System.Drawing.Size(75, 23);
            this.btn_Back.TabIndex = 5;
            this.btn_Back.Text = "<<";
            this.btn_Back.UseVisualStyleBackColor = true;
            this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(93, 245);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.btn_Delete.TabIndex = 6;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_Forward
            // 
            this.btn_Forward.Location = new System.Drawing.Point(174, 245);
            this.btn_Forward.Name = "btn_Forward";
            this.btn_Forward.Size = new System.Drawing.Size(75, 23);
            this.btn_Forward.TabIndex = 7;
            this.btn_Forward.Text = ">>";
            this.btn_Forward.UseVisualStyleBackColor = true;
            this.btn_Forward.Click += new System.EventHandler(this.btn_Forward_Click);
            // 
            // frmSticker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 289);
            this.Controls.Add(this.btn_Forward);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Back);
            this.Controls.Add(this.btnAddToDictonary);
            this.Controls.Add(this.txtTranslate);
            this.Controls.Add(this.txtWord);
            this.Controls.Add(this.lb_translate);
            this.Controls.Add(this.lb_word);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSticker";
            this.ShowInTaskbar = false;
            this.Text = "LinguaLeoSticker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSticker_FormClosing);
            this.Load += new System.EventHandler(this.frmSticker_Load);
            this.DoubleClick += new System.EventHandler(this.frmSticker_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmSticker_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_word;
        private System.Windows.Forms.Label lb_translate;
        private System.Windows.Forms.Timer tmrChangeWord;
        private System.Windows.Forms.TextBox txtWord;
        private System.Windows.Forms.TextBox txtTranslate;
        private System.Windows.Forms.Button btnAddToDictonary;
        private System.Windows.Forms.Button btn_Back;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.Button btn_Forward;
    }
}

