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

            richTextBox1.BackColor = System.Drawing.Color.FromArgb(30, 30, 44);
            richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;

            label4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 19.8F,
                                   System.Drawing.FontStyle.Bold);

            label7.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label7.BackColor = System.Drawing.Color.Transparent;

            label1.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label1.BackColor = System.Drawing.Color.Transparent;

            label2.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label2.BackColor = System.Drawing.Color.Transparent;

            textBox1.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
            textBox1.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            textBox2.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
            textBox2.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            label3.ForeColor = System.Drawing.Color.FromArgb(242, 159, 103);
            label3.BackColor = System.Drawing.Color.Transparent;

            label5.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label5.BackColor = System.Drawing.Color.Transparent;

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
            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to the database.\n\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            string email = Microsoft.VisualBasic.Interaction.InputBox( //ask the user for their registered email address
                "Enter your registered email address:",
                "Forgot Password",
                "");

            if (string.IsNullOrWhiteSpace(email))
                return;

            try
            {
                string username = DatabaseHelper.GetUsernameByEmail(email);

                if (username != null)
                {
                    MessageBox.Show(
                        $"An account with the username \"{username}\" was found.\n\n" +
                        "Please contact your system administrator to reset your password.",
                        "Account Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        "No account was found with that email address.",
                        "Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Database error: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both your email and password.",
                    "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (DatabaseHelper.ValidateUser(email, password))
                {
                    DASHBOARD dashboard = new DASHBOARD();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid email or password. Please try again.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Clear();
                    textBox2.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            REGISTRATION regForm = new REGISTRATION();
            regForm.Show();
            this.Hide();
        }
    }
}
