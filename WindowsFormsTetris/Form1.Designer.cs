namespace WindowsFormsTetris
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerFall = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.pictureBoxNext = new System.Windows.Forms.PictureBox();
            this.timerControl = new System.Windows.Forms.Timer(this.components);
            this.labelScore = new System.Windows.Forms.Label();
            this.labelHowTo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNext)).BeginInit();
            this.SuspendLayout();
            // 
            // timerFall
            // 
            this.timerFall.Enabled = true;
            this.timerFall.Interval = 150;
            this.timerFall.Tick += new System.EventHandler(this.timerFall_Tick);
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackColor = System.Drawing.SystemColors.Desktop;
            this.pictureBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxMain.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(230, 374);
            this.pictureBoxMain.TabIndex = 0;
            this.pictureBoxMain.TabStop = false;
            // 
            // pictureBoxNext
            // 
            this.pictureBoxNext.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxNext.Location = new System.Drawing.Point(262, 12);
            this.pictureBoxNext.Name = "pictureBoxNext";
            this.pictureBoxNext.Size = new System.Drawing.Size(57, 56);
            this.pictureBoxNext.TabIndex = 1;
            this.pictureBoxNext.TabStop = false;
            // 
            // timerControl
            // 
            this.timerControl.Enabled = true;
            this.timerControl.Interval = 50;
            this.timerControl.Tick += new System.EventHandler(this.timerControl_Tick);
            // 
            // labelScore
            // 
            this.labelScore.AutoSize = true;
            this.labelScore.Location = new System.Drawing.Point(260, 97);
            this.labelScore.Name = "labelScore";
            this.labelScore.Size = new System.Drawing.Size(35, 12);
            this.labelScore.TabIndex = 2;
            this.labelScore.Text = "label1";
            // 
            // labelHowTo
            // 
            this.labelHowTo.AutoSize = true;
            this.labelHowTo.Location = new System.Drawing.Point(262, 134);
            this.labelHowTo.Name = "labelHowTo";
            this.labelHowTo.Size = new System.Drawing.Size(35, 12);
            this.labelHowTo.TabIndex = 3;
            this.labelHowTo.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 398);
            this.Controls.Add(this.labelHowTo);
            this.Controls.Add(this.labelScore);
            this.Controls.Add(this.pictureBoxNext);
            this.Controls.Add(this.pictureBoxMain);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Tetris";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNext)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerFall;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private System.Windows.Forms.PictureBox pictureBoxNext;
        private System.Windows.Forms.Timer timerControl;
        private System.Windows.Forms.Label labelScore;
        private System.Windows.Forms.Label labelHowTo;
    }
}

