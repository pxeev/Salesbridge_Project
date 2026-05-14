using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salesbridge
{
    public partial class LOGIN : Form
    {
        public LOGIN()
        {
            InitializeComponent();
            ApplyTheme();
        }
        private void ApplyTheme()  // LOGIN
        {
            this.BackColor = System.Drawing.Color.FromArgb(245, 244, 240);

            // Dark left panel → navy
            richTextBox1.BackColor = System.Drawing.Color.FromArgb(30, 30, 44);
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // Bridge logo
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            // "Welcome back!" (label4)
            label4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 19.8F,
                                   System.Drawing.FontStyle.Bold);

            // "Login to your Salesbridge account" (label7)
            label7.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label7.BackColor = System.Drawing.Color.Transparent;

            // "Email:" (label1)
            label1.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label1.BackColor = System.Drawing.Color.Transparent;

            // "Password:" (label2)
            label2.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label2.BackColor = System.Drawing.Color.Transparent;

            // Email TextBox (textBox1)
            textBox1.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
            textBox1.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Password TextBox (textBox2)
            textBox2.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
            textBox2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // "Forgot Password" (label3)
            label3.ForeColor = System.Drawing.Color.FromArgb(242, 159, 103);
            label3.BackColor = System.Drawing.Color.Transparent;

            // "Don't have an account yet?" (label5)
            label5.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label5.BackColor = System.Drawing.Color.Transparent;

            // Continue (button1) — orange
            button1.BackColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button1.ForeColor = System.Drawing.Color.White;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new System.Drawing.Font("Segoe UI", 10.2F,
                                     System.Drawing.FontStyle.Bold);
            button1.Cursor = System.Windows.Forms.Cursors.Hand;
            button1.UseVisualStyleBackColor = false;

            // Register (button2) — orange outline
            button2.BackColor = System.Drawing.Color.Transparent;
            button2.ForeColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 1;
            button2.FlatAppearance.BorderColor =
                System.Drawing.Color.FromArgb(242, 159, 103);
            button2.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            button2.Cursor = System.Windows.Forms.Cursors.Hand;
            button2.UseVisualStyleBackColor = false;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void LOGIN_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
