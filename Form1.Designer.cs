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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerField));
            pictureBoxRed = new PictureBox();
            pictureBoxBlue = new PictureBox();
            pictureBoxOrange = new PictureBox();
            pictureBoxGreen = new PictureBox();
            BTN_Start = new Button();
            richTextBoxShowNumbersOfSequences = new ReadOnlyRichTextBox();
            richTextBoxTurn = new ReadOnlyRichTextBox();
            richTextBoxShowRounds = new ReadOnlyRichTextBox();
            richTextBoxShowLevelName = new ReadOnlyRichTextBox();
            richTextBoxShowLevelNumber = new ReadOnlyRichTextBox();
            LabelClickHere1 = new Label();
            LabelClickHere2 = new Label();
            LabelClickHere3 = new Label();
            LabelClickHere4 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxRed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBlue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOrange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGreen).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxRed
            // 
            pictureBoxRed.Location = new Point(13, 12);
            pictureBoxRed.Margin = new Padding(4, 3, 4, 3);
            pictureBoxRed.Name = "pictureBoxRed";
            pictureBoxRed.Size = new Size(300, 294);
            pictureBoxRed.TabIndex = 3;
            pictureBoxRed.TabStop = false;
            // 
            // pictureBoxBlue
            // 
            pictureBoxBlue.Location = new Point(321, 14);
            pictureBoxBlue.Margin = new Padding(4, 3, 4, 3);
            pictureBoxBlue.Name = "pictureBoxBlue";
            pictureBoxBlue.Size = new Size(300, 294);
            pictureBoxBlue.TabIndex = 4;
            pictureBoxBlue.TabStop = false;
            // 
            // pictureBoxOrange
            // 
            pictureBoxOrange.Location = new Point(14, 316);
            pictureBoxOrange.Margin = new Padding(4, 3, 4, 3);
            pictureBoxOrange.Name = "pictureBoxOrange";
            pictureBoxOrange.Size = new Size(300, 294);
            pictureBoxOrange.TabIndex = 5;
            pictureBoxOrange.TabStop = false;
            // 
            // pictureBoxGreen
            // 
            pictureBoxGreen.Location = new Point(321, 316);
            pictureBoxGreen.Margin = new Padding(4, 3, 4, 3);
            pictureBoxGreen.Name = "pictureBoxGreen";
            pictureBoxGreen.Size = new Size(300, 294);
            pictureBoxGreen.TabIndex = 6;
            pictureBoxGreen.TabStop = false;
            // 
            // BTN_Start
            // 
            BTN_Start.BackColor = Color.Lime;
            BTN_Start.Cursor = Cursors.Hand;
            BTN_Start.FlatStyle = FlatStyle.Popup;
            BTN_Start.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            BTN_Start.Location = new Point(243, 277);
            BTN_Start.Margin = new Padding(4, 3, 4, 3);
            BTN_Start.Name = "BTN_Start";
            BTN_Start.Size = new Size(153, 70);
            BTN_Start.TabIndex = 0;
            BTN_Start.Text = "Start";
            BTN_Start.UseVisualStyleBackColor = false;
            BTN_Start.Click += BTN_Start_Click;
            // 
            // richTextBoxShowNumbersOfSequences
            // 
            richTextBoxShowNumbersOfSequences.BackColor = Color.Yellow;
            richTextBoxShowNumbersOfSequences.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxShowNumbersOfSequences.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            richTextBoxShowNumbersOfSequences.ImeMode = ImeMode.NoControl;
            richTextBoxShowNumbersOfSequences.Location = new Point(242, 343);
            richTextBoxShowNumbersOfSequences.Margin = new Padding(4, 3, 4, 3);
            richTextBoxShowNumbersOfSequences.Name = "richTextBoxShowNumbersOfSequences";
            richTextBoxShowNumbersOfSequences.ReadOnly = true;
            richTextBoxShowNumbersOfSequences.ShortcutsEnabled = false;
            richTextBoxShowNumbersOfSequences.Size = new Size(154, 27);
            richTextBoxShowNumbersOfSequences.TabIndex = 1;
            richTextBoxShowNumbersOfSequences.Text = " Keep Your Focus";
            // 
            // richTextBoxTurn
            // 
            richTextBoxTurn.BackColor = Color.Salmon;
            richTextBoxTurn.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxTurn.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            richTextBoxTurn.Location = new Point(242, 277);
            richTextBoxTurn.Margin = new Padding(4, 3, 4, 3);
            richTextBoxTurn.Name = "richTextBoxTurn";
            richTextBoxTurn.ReadOnly = true;
            richTextBoxTurn.Size = new Size(154, 70);
            richTextBoxTurn.TabIndex = 2;
            richTextBoxTurn.Text = "";
            // 
            // richTextBoxShowRounds
            // 
            richTextBoxShowRounds.BackColor = Color.LightSkyBlue;
            richTextBoxShowRounds.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxShowRounds.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            richTextBoxShowRounds.Location = new Point(243, 376);
            richTextBoxShowRounds.Margin = new Padding(4, 3, 4, 3);
            richTextBoxShowRounds.Multiline = false;
            richTextBoxShowRounds.Name = "richTextBoxShowRounds";
            richTextBoxShowRounds.ReadOnly = true;
            richTextBoxShowRounds.Size = new Size(154, 27);
            richTextBoxShowRounds.TabIndex = 7;
            richTextBoxShowRounds.Text = "";
            // 
            // richTextBoxShowLevelName
            // 
            richTextBoxShowLevelName.BackColor = Color.LightSkyBlue;
            richTextBoxShowLevelName.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxShowLevelName.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            richTextBoxShowLevelName.Location = new Point(293, 244);
            richTextBoxShowLevelName.Margin = new Padding(4, 3, 4, 3);
            richTextBoxShowLevelName.Multiline = false;
            richTextBoxShowLevelName.Name = "richTextBoxShowLevelName";
            richTextBoxShowLevelName.ReadOnly = true;
            richTextBoxShowLevelName.Size = new Size(104, 27);
            richTextBoxShowLevelName.TabIndex = 8;
            richTextBoxShowLevelName.Text = "";
            // 
            // richTextBoxShowLevelNumber
            // 
            richTextBoxShowLevelNumber.BackColor = Color.LightSkyBlue;
            richTextBoxShowLevelNumber.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxShowLevelNumber.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            richTextBoxShowLevelNumber.ForeColor = SystemColors.WindowText;
            richTextBoxShowLevelNumber.Location = new Point(242, 244);
            richTextBoxShowLevelNumber.Multiline = false;
            richTextBoxShowLevelNumber.Name = "richTextBoxShowLevelNumber";
            richTextBoxShowLevelNumber.ReadOnly = true;
            richTextBoxShowLevelNumber.Size = new Size(44, 27);
            richTextBoxShowLevelNumber.TabIndex = 9;
            richTextBoxShowLevelNumber.Text = "";
            // 
            // LabelClickHere1
            // 
            LabelClickHere1.AutoSize = true;
            LabelClickHere1.BackColor = Color.Transparent;
            LabelClickHere1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelClickHere1.ForeColor = SystemColors.ButtonHighlight;
            LabelClickHere1.Location = new Point(102, 140);
            LabelClickHere1.Name = "LabelClickHere1";
            LabelClickHere1.Size = new Size(0, 20);
            LabelClickHere1.TabIndex = 10;
            LabelClickHere1.TextAlign = ContentAlignment.MiddleCenter;
            LabelClickHere1.Visible = false;
            // 
            // LabelClickHere2
            // 
            LabelClickHere2.AutoSize = true;
            LabelClickHere2.BackColor = Color.Transparent;
            LabelClickHere2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelClickHere2.ForeColor = SystemColors.ButtonHighlight;
            LabelClickHere2.Location = new Point(439, 140);
            LabelClickHere2.Name = "LabelClickHere2";
            LabelClickHere2.Size = new Size(0, 20);
            LabelClickHere2.TabIndex = 11;
            LabelClickHere2.TextAlign = ContentAlignment.MiddleCenter;
            LabelClickHere2.Visible = false;
            // 
            // LabelClickHere3
            // 
            LabelClickHere3.AutoSize = true;
            LabelClickHere3.BackColor = Color.Transparent;
            LabelClickHere3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelClickHere3.ForeColor = SystemColors.ButtonHighlight;
            LabelClickHere3.Location = new Point(102, 461);
            LabelClickHere3.Name = "LabelClickHere3";
            LabelClickHere3.Size = new Size(0, 20);
            LabelClickHere3.TabIndex = 12;
            LabelClickHere3.TextAlign = ContentAlignment.MiddleCenter;
            LabelClickHere3.Visible = false;
            // 
            // LabelClickHere4
            // 
            LabelClickHere4.AutoSize = true;
            LabelClickHere4.BackColor = Color.Transparent;
            LabelClickHere4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelClickHere4.ForeColor = SystemColors.ButtonHighlight;
            LabelClickHere4.Location = new Point(439, 461);
            LabelClickHere4.Name = "LabelClickHere4";
            LabelClickHere4.Size = new Size(0, 20);
            LabelClickHere4.TabIndex = 13;
            LabelClickHere4.TextAlign = ContentAlignment.MiddleCenter;
            LabelClickHere4.Visible = false;
            // 
            // PlayerField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            ClientSize = new Size(631, 668);
            Controls.Add(LabelClickHere4);
            Controls.Add(LabelClickHere3);
            Controls.Add(LabelClickHere2);
            Controls.Add(LabelClickHere1);
            Controls.Add(BTN_Start);
            Controls.Add(richTextBoxShowLevelNumber);
            Controls.Add(richTextBoxShowLevelName);
            Controls.Add(richTextBoxShowRounds);
            Controls.Add(richTextBoxShowNumbersOfSequences);
            Controls.Add(richTextBoxTurn);
            Controls.Add(pictureBoxGreen);
            Controls.Add(pictureBoxOrange);
            Controls.Add(pictureBoxBlue);
            Controls.Add(pictureBoxRed);
            Font = new Font("Segoe UI Symbol", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlayerField";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Keep Your Focus";
            ((System.ComponentModel.ISupportInitialize)pictureBoxRed).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBlue).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOrange).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGreen).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button BTN_Start;

        private ReadOnlyRichTextBox richTextBoxShowNumbersOfSequences;
        private ReadOnlyRichTextBox richTextBoxTurn;
        private ReadOnlyRichTextBox richTextBoxShowRounds;
        private ReadOnlyRichTextBox richTextBoxShowLevelName;
        private ReadOnlyRichTextBox richTextBoxShowLevelNumber;

        private System.Windows.Forms.PictureBox pictureBoxRed;
        private System.Windows.Forms.PictureBox pictureBoxBlue;
        private System.Windows.Forms.PictureBox pictureBoxOrange;
        private System.Windows.Forms.PictureBox pictureBoxGreen;
        private Label LabelClickHere1;
        private Label LabelClickHere2;
        private Label LabelClickHere3;
        private Label LabelClickHere4;
        //private RichTextBox richTextBoxLevelNumber;
    }
}
