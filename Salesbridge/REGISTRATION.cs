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
    public partial class REGISTRATION : Form
    {
        public REGISTRATION()
        {
            InitializeComponent();
        }
        private void ApplyTheme()  // REGISTRATION THEME
        {
            this.BackColor = System.Drawing.Color.FromArgb(245, 244, 240);

            richTextBox1.BackColor = System.Drawing.Color.FromArgb(30, 30, 44);
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            label4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 19.8F,
                                   System.Drawing.FontStyle.Bold);

            foreach (var lbl in new[] { label1, label2, label3, label5 })
            {
                lbl.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
                lbl.BackColor = System.Drawing.Color.Transparent;
            }

            label6.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label6.BackColor = System.Drawing.Color.Transparent;

            foreach (var tb in new System.Windows.Forms.TextBox[]
                { textBox1, textBox2, textBox3, textBox4 })
            {
                tb.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
                tb.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
                tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            }

            button1.BackColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button1.ForeColor = System.Drawing.Color.White;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new System.Drawing.Font("Segoe UI", 10.2F,
                                     System.Drawing.FontStyle.Bold);
            button1.Cursor = System.Windows.Forms.Cursors.Hand;
            button1.UseVisualStyleBackColor = false;

            button2.BackColor = System.Drawing.Color.Transparent;
            button2.ForeColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            button2.Cursor = System.Windows.Forms.Cursors.Hand;
            button2.UseVisualStyleBackColor = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void REGISTRATION_Load(object sender, EventArgs e)
        {

        }
    }
}
