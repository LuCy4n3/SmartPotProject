using System.ComponentModel;
using System.Windows.Forms;

namespace ProgrammerUI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
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
            this.button1 = new System.Windows.Forms.Button();
            this.lblFile = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnReadStatuses = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(211, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 45);
            this.button1.TabIndex = 0;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(384, 36);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(80, 25);
            this.lblFile.TabIndex = 1;
            this.lblFile.Text = "No File";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.lblFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2246, 100);
            this.panel1.TabIndex = 2;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1275, 31);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(122, 42);
            this.button6.TabIndex = 7;
            this.button6.Text = "Restart";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1055, 37);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(140, 31);
            this.textBox1.TabIndex = 5;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(778, 24);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(206, 46);
            this.button4.TabIndex = 4;
            this.button4.Text = "Set dimming";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(497, 22);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 49);
            this.button3.TabIndex = 3;
            this.button3.Text = "Test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 26);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 49);
            this.button2.TabIndex = 2;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.InactiveBorder;
            this.richTextBox1.Location = new System.Drawing.Point(0, 100);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(2246, 1113);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(43, 535);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(154, 52);
            this.button5.TabIndex = 6;
            this.button5.Text = "Send Lights";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnReadStatuses);
            this.panel2.Controls.Add(this.button8);
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.textBox9);
            this.panel2.Controls.Add(this.button5);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.textBox8);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.textBox7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.textBox6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.textBox5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.textBox4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(2015, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(231, 1113);
            this.panel2.TabIndex = 4;
            // 
            // btnReadStatuses
            // 
            this.btnReadStatuses.Location = new System.Drawing.Point(31, 750);
            this.btnReadStatuses.Name = "btnReadStatuses";
            this.btnReadStatuses.Size = new System.Drawing.Size(166, 73);
            this.btnReadStatuses.TabIndex = 18;
            this.btnReadStatuses.Text = "Read Statuses";
            this.btnReadStatuses.UseVisualStyleBackColor = true;
            this.btnReadStatuses.Click += new System.EventHandler(this.btnReadStatuses_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(61, 861);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(103, 46);
            this.button8.TabIndex = 17;
            this.button8.Text = "All On";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(61, 656);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(117, 59);
            this.button7.TabIndex = 16;
            this.button7.Text = "All Off";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(101, 435);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(100, 31);
            this.textBox9.TabIndex = 15;
            this.textBox9.Tag = "7";
            this.textBox9.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(38, 435);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 25);
            this.label8.TabIndex = 14;
            this.label8.Text = "L7";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(101, 372);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(100, 31);
            this.textBox8.TabIndex = 13;
            this.textBox8.Tag = "6";
            this.textBox8.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 372);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 25);
            this.label7.TabIndex = 12;
            this.label7.Text = "L6";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(101, 311);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(100, 31);
            this.textBox7.TabIndex = 11;
            this.textBox7.Tag = "5";
            this.textBox7.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 311);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 25);
            this.label6.TabIndex = 10;
            this.label6.Text = "L5";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(101, 260);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 31);
            this.textBox6.TabIndex = 9;
            this.textBox6.Tag = "4";
            this.textBox6.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 260);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "L4";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(101, 205);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 31);
            this.textBox5.TabIndex = 7;
            this.textBox5.Tag = "3";
            this.textBox5.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 205);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "L3";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(101, 153);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 31);
            this.textBox4.TabIndex = 5;
            this.textBox4.Tag = "2";
            this.textBox4.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "L2";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(101, 88);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 31);
            this.textBox3.TabIndex = 3;
            this.textBox3.Tag = "1";
            this.textBox3.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "L1";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(101, 27);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 31);
            this.textBox2.TabIndex = 1;
            this.textBox2.Tag = "0";
            this.textBox2.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "L0";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(1521, 27);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(123, 51);
            this.button9.TabIndex = 8;
            this.button9.Text = "Rts Test";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2246, 1213);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Programmer UI";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button button1;
        private Label lblFile;
        private Panel panel1;
        private Button button2;
        private RichTextBox richTextBox1;
        private Button button3;
        private TextBox textBox1;
        private Button button4;
        private Button button5;
        private Button button6;
        private Panel panel2;
        private TextBox textBox9;
        private Label label8;
        private TextBox textBox8;
        private Label label7;
        private TextBox textBox7;
        private Label label6;
        private TextBox textBox6;
        private Label label5;
        private TextBox textBox5;
        private Label label4;
        private TextBox textBox4;
        private Label label3;
        private TextBox textBox3;
        private Label label2;
        private TextBox textBox2;
        private Label label1;
        private Button button7;
        private Button button8;
        private Button btnReadStatuses;
        private Button button9;
    }
}

