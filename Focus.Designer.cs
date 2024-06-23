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
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            startBTN = new Button();
            richTextBoxShowNumbersOfSequences = new ReadOnlyRichTextBox();
            richTextBoxTurn = new ReadOnlyRichTextBox();
            richTextBoxShowRounds = new ReadOnlyRichTextBox();
            richTextBoxShowLevelName = new ReadOnlyRichTextBox();
            richTextBoxShowLevelNumber = new ReadOnlyRichTextBox();
            LabelMessage1 = new Label();
            LabelMessage2 = new Label();
            LabelMessage3 = new Label();
            LabelMessage4 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(13, 12);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(300, 294);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(321, 14);
            pictureBox2.Margin = new Padding(4, 3, 4, 3);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(300, 294);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(14, 316);
            pictureBox3.Margin = new Padding(4, 3, 4, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(300, 294);
            pictureBox3.TabIndex = 5;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(321, 316);
            pictureBox4.Margin = new Padding(4, 3, 4, 3);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(300, 294);
            pictureBox4.TabIndex = 6;
            pictureBox4.TabStop = false;
            // 
            // startBTN
            // 
            startBTN.BackColor = Color.Lime;
            startBTN.Cursor = Cursors.Hand;
            startBTN.FlatStyle = FlatStyle.Popup;
            startBTN.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            startBTN.Location = new Point(243, 277);
            startBTN.Margin = new Padding(4, 3, 4, 3);
            startBTN.Name = "startBTN";
            startBTN.Size = new Size(153, 70);
            startBTN.TabIndex = 0;
            startBTN.Text = "Start";
            startBTN.UseVisualStyleBackColor = false;
            startBTN.Click += StartButtonClick;
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
            // LabelMessage1
            // 
            LabelMessage1.AutoSize = true;
            LabelMessage1.BackColor = Color.Transparent;
            LabelMessage1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelMessage1.ForeColor = SystemColors.ButtonHighlight;
            LabelMessage1.Location = new Point(102, 140);
            LabelMessage1.Name = "LabelMessage1";
            LabelMessage1.Size = new Size(0, 20);
            LabelMessage1.TabIndex = 10;
            LabelMessage1.TextAlign = ContentAlignment.MiddleCenter;
            LabelMessage1.Visible = false;
            // 
            // LabelMessage2
            // 
            LabelMessage2.AutoSize = true;
            LabelMessage2.BackColor = Color.Transparent;
            LabelMessage2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelMessage2.ForeColor = SystemColors.ButtonHighlight;
            LabelMessage2.Location = new Point(439, 140);
            LabelMessage2.Name = "LabelMessage2";
            LabelMessage2.Size = new Size(0, 20);
            LabelMessage2.TabIndex = 11;
            LabelMessage2.TextAlign = ContentAlignment.MiddleCenter;
            LabelMessage2.Visible = false;
            // 
            // LabelMessage3
            // 
            LabelMessage3.AutoSize = true;
            LabelMessage3.BackColor = Color.Transparent;
            LabelMessage3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelMessage3.ForeColor = SystemColors.ButtonHighlight;
            LabelMessage3.Location = new Point(102, 461);
            LabelMessage3.Name = "LabelMessage3";
            LabelMessage3.Size = new Size(0, 20);
            LabelMessage3.TabIndex = 12;
            LabelMessage3.TextAlign = ContentAlignment.MiddleCenter;
            LabelMessage3.Visible = false;
            // 
            // LabelMessage4
            // 
            LabelMessage4.AutoSize = true;
            LabelMessage4.BackColor = Color.Transparent;
            LabelMessage4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            LabelMessage4.ForeColor = SystemColors.ButtonHighlight;
            LabelMessage4.Location = new Point(439, 461);
            LabelMessage4.Name = "LabelMessage4";
            LabelMessage4.Size = new Size(0, 20);
            LabelMessage4.TabIndex = 13;
            LabelMessage4.TextAlign = ContentAlignment.MiddleCenter;
            LabelMessage4.Visible = false;
            // 
            // PlayerField
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            ClientSize = new Size(631, 668);
            Controls.Add(LabelMessage4);
            Controls.Add(LabelMessage3);
            Controls.Add(LabelMessage2);
            Controls.Add(LabelMessage1);
            Controls.Add(startBTN);
            Controls.Add(richTextBoxShowLevelNumber);
            Controls.Add(richTextBoxShowLevelName);
            Controls.Add(richTextBoxShowRounds);
            Controls.Add(richTextBoxShowNumbersOfSequences);
            Controls.Add(richTextBoxTurn);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Font = new Font("Segoe UI Symbol", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlayerField";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Keep Your Focus";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
