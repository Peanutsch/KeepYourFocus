using System.Windows.Forms;

namespace KeepYourFocus
{
    partial class PlayerField
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerField));
            this.pictureBox1 = new PictureBox();
            this.pictureBox2 = new PictureBox();
            this.pictureBox3 = new PictureBox();
            this.pictureBox4 = new PictureBox();
            this.startBTN = new Button();
            this.richTextBoxShowNumbersOfSequences = new ReadOnlyRichTextBox();
            this.richTextBoxTurn = new ReadOnlyRichTextBox();
            this.richTextBoxShowRounds = new ReadOnlyRichTextBox();
            this.richTextBoxShowLevelName = new ReadOnlyRichTextBox();
            this.richTextBoxShowLevelNumber = new ReadOnlyRichTextBox();
            this.LabelMessage1 = new Label();
            this.LabelMessage2 = new Label();
            this.LabelMessage3 = new Label();
            this.LabelMessage4 = new Label();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox4).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Point(13, 12);
            this.pictureBox1.Margin = new Padding(4, 4, 4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(300, 294);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new Point(321, 14);
            this.pictureBox2.Margin = new Padding(4, 4, 4, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(300, 294);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new Point(14, 316);
            this.pictureBox3.Margin = new Padding(4, 4, 4, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new Size(300, 294);
            this.pictureBox3.TabIndex = 5;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new Point(321, 316);
            this.pictureBox4.Margin = new Padding(4, 4, 4, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new Size(300, 294);
            this.pictureBox4.TabIndex = 6;
            this.pictureBox4.TabStop = false;
            // 
            // startBTN
            // 
            this.startBTN.BackColor = Color.Lime;
            this.startBTN.Cursor = Cursors.Hand;
            this.startBTN.FlatStyle = FlatStyle.Popup;
            this.startBTN.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.startBTN.Location = new Point(243, 277);
            this.startBTN.Margin = new Padding(4, 3, 4, 3);
            this.startBTN.Name = "startBTN";
            this.startBTN.Size = new Size(153, 70);
            this.startBTN.TabIndex = 0;
            this.startBTN.Text = "Start";
            this.startBTN.UseVisualStyleBackColor = false;
            this.startBTN.Click += this.StartButtonClick;
            // 
            // richTextBoxShowNumbersOfSequences
            // 
            this.richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            this.richTextBoxShowNumbersOfSequences.BorderStyle = BorderStyle.FixedSingle;
            this.richTextBoxShowNumbersOfSequences.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.richTextBoxShowNumbersOfSequences.ImeMode = ImeMode.NoControl;
            this.richTextBoxShowNumbersOfSequences.Location = new Point(242, 343);
            this.richTextBoxShowNumbersOfSequences.Margin = new Padding(4, 3, 4, 3);
            this.richTextBoxShowNumbersOfSequences.Name = "richTextBoxShowNumbersOfSequences";
            this.richTextBoxShowNumbersOfSequences.ReadOnly = true;
            this.richTextBoxShowNumbersOfSequences.ShortcutsEnabled = false;
            this.richTextBoxShowNumbersOfSequences.Size = new Size(154, 27);
            this.richTextBoxShowNumbersOfSequences.TabIndex = 1;
            this.richTextBoxShowNumbersOfSequences.Text = " Keep Your Focus";
            // 
            // richTextBoxTurn
            // 
            this.richTextBoxTurn.BackColor = Color.Salmon;
            this.richTextBoxTurn.BorderStyle = BorderStyle.FixedSingle;
            this.richTextBoxTurn.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.richTextBoxTurn.Location = new Point(242, 277);
            this.richTextBoxTurn.Margin = new Padding(4, 3, 4, 3);
            this.richTextBoxTurn.Name = "richTextBoxTurn";
            this.richTextBoxTurn.ReadOnly = true;
            this.richTextBoxTurn.Size = new Size(154, 70);
            this.richTextBoxTurn.TabIndex = 2;
            this.richTextBoxTurn.Text = "";
            // 
            // richTextBoxShowRounds
            // 
            this.richTextBoxShowRounds.BackColor = Color.LightSkyBlue;
            this.richTextBoxShowRounds.BorderStyle = BorderStyle.FixedSingle;
            this.richTextBoxShowRounds.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.richTextBoxShowRounds.Location = new Point(243, 376);
            this.richTextBoxShowRounds.Margin = new Padding(4, 3, 4, 3);
            this.richTextBoxShowRounds.Multiline = false;
            this.richTextBoxShowRounds.Name = "richTextBoxShowRounds";
            this.richTextBoxShowRounds.ReadOnly = true;
            this.richTextBoxShowRounds.Size = new Size(154, 27);
            this.richTextBoxShowRounds.TabIndex = 7;
            this.richTextBoxShowRounds.Text = "";
            // 
            // richTextBoxShowLevelName
            // 
            this.richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;
            this.richTextBoxShowLevelName.BorderStyle = BorderStyle.FixedSingle;
            this.richTextBoxShowLevelName.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.richTextBoxShowLevelName.Location = new Point(293, 244);
            this.richTextBoxShowLevelName.Margin = new Padding(4, 3, 4, 3);
            this.richTextBoxShowLevelName.Multiline = false;
            this.richTextBoxShowLevelName.Name = "richTextBoxShowLevelName";
            this.richTextBoxShowLevelName.ReadOnly = true;
            this.richTextBoxShowLevelName.Size = new Size(104, 27);
            this.richTextBoxShowLevelName.TabIndex = 8;
            this.richTextBoxShowLevelName.Text = "";
            // 
            // richTextBoxShowLevelNumber
            // 
            this.richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;
            this.richTextBoxShowLevelNumber.BorderStyle = BorderStyle.FixedSingle;
            this.richTextBoxShowLevelNumber.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.richTextBoxShowLevelNumber.ForeColor = SystemColors.WindowText;
            this.richTextBoxShowLevelNumber.Location = new Point(242, 244);
            this.richTextBoxShowLevelNumber.Multiline = false;
            this.richTextBoxShowLevelNumber.Name = "richTextBoxShowLevelNumber";
            this.richTextBoxShowLevelNumber.ReadOnly = true;
            this.richTextBoxShowLevelNumber.Size = new Size(44, 27);
            this.richTextBoxShowLevelNumber.TabIndex = 9;
            this.richTextBoxShowLevelNumber.Text = "";
            // 
            // LabelMessage1
            // 
            this.LabelMessage1.AutoSize = true;
            this.LabelMessage1.BackColor = Color.Transparent;
            this.LabelMessage1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.LabelMessage1.ForeColor = SystemColors.ButtonHighlight;
            this.LabelMessage1.Location = new Point(112, 150);
            this.LabelMessage1.Name = "LabelMessage1";
            this.LabelMessage1.Size = new Size(0, 20);
            this.LabelMessage1.TabIndex = 10;
            this.LabelMessage1.TextAlign = ContentAlignment.MiddleCenter;
            this.LabelMessage1.Visible = false;
            // 
            // LabelMessage2
            // 
            this.LabelMessage2.AutoSize = true;
            this.LabelMessage2.BackColor = Color.Transparent;
            this.LabelMessage2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.LabelMessage2.ForeColor = SystemColors.ButtonHighlight;
            this.LabelMessage2.Location = new Point(449, 150);
            this.LabelMessage2.Name = "LabelMessage2";
            this.LabelMessage2.Size = new Size(0, 20);
            this.LabelMessage2.TabIndex = 11;
            this.LabelMessage2.TextAlign = ContentAlignment.MiddleCenter;
            this.LabelMessage2.Visible = false;
            // 
            // LabelMessage3
            // 
            this.LabelMessage3.AutoSize = true;
            this.LabelMessage3.BackColor = Color.Transparent;
            this.LabelMessage3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.LabelMessage3.ForeColor = SystemColors.ButtonHighlight;
            this.LabelMessage3.Location = new Point(112, 471);
            this.LabelMessage3.Name = "LabelMessage3";
            this.LabelMessage3.Size = new Size(0, 20);
            this.LabelMessage3.TabIndex = 12;
            this.LabelMessage3.TextAlign = ContentAlignment.MiddleCenter;
            this.LabelMessage3.Visible = false;
            // 
            // LabelMessage4
            // 
            this.LabelMessage4.AutoSize = true;
            this.LabelMessage4.BackColor = Color.Transparent;
            this.LabelMessage4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.LabelMessage4.ForeColor = SystemColors.ButtonHighlight;
            this.LabelMessage4.Location = new Point(449, 471);
            this.LabelMessage4.Name = "LabelMessage4";
            this.LabelMessage4.Size = new Size(0, 20);
            this.LabelMessage4.TabIndex = 13;
            this.LabelMessage4.TextAlign = ContentAlignment.MiddleCenter;
            this.LabelMessage4.Visible = false;
            // 
            // PlayerField
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.BackColor = Color.Black;
            this.ClientSize = new Size(631, 668);
            this.Controls.Add(this.LabelMessage4);
            this.Controls.Add(this.LabelMessage3);
            this.Controls.Add(this.LabelMessage2);
            this.Controls.Add(this.LabelMessage1);
            this.Controls.Add(this.startBTN);
            this.Controls.Add(this.richTextBoxShowLevelNumber);
            this.Controls.Add(this.richTextBoxShowLevelName);
            this.Controls.Add(this.richTextBoxShowRounds);
            this.Controls.Add(this.richTextBoxShowNumbersOfSequences);
            this.Controls.Add(this.richTextBoxTurn);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Font = new Font("Segoe UI Symbol", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.Margin = new Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlayerField";
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Keep Your Focus";
            ((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox4).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button startBTN;

        private ReadOnlyRichTextBox richTextBoxShowNumbersOfSequences;
        private ReadOnlyRichTextBox richTextBoxTurn;
        private ReadOnlyRichTextBox richTextBoxShowRounds;
        private ReadOnlyRichTextBox richTextBoxShowLevelName;
        private ReadOnlyRichTextBox richTextBoxShowLevelNumber;

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private Label LabelMessage1;
        private Label LabelMessage2;
        private Label LabelMessage3;
        private Label LabelMessage4;
        //private RichTextBox richTextBoxLevelNumber;
    }
}
