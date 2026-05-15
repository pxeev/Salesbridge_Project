using System;
using System.Windows.Forms;
namespace Salesbridge
{
    public partial class REGISTRATION : Form
    {
        public REGISTRATION()
        {
            InitializeComponent();
        }
        private void ApplyTheme()
        {
            this.BackColor = System.Drawing.Color.FromArgb(245, 244, 240);
            richTextBox1.BackColor = System.Drawing.Color.FromArgb(30, 30, 44);
            richTextBox1.BorderStyle = BorderStyle.None;
            pictureBox1.BackColor = System.Drawing.Color.Transparent;
            pictureBox1.BorderStyle = BorderStyle.None;
            label4.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
            label4.BackColor = System.Drawing.Color.Transparent;
            label4.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold);

            foreach (var lbl in new[] { label1, label2, label3, label5 })
            {
                lbl.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
                lbl.BackColor = System.Drawing.Color.Transparent;
            }
            label6.ForeColor = System.Drawing.Color.FromArgb(140, 140, 155);
            label6.BackColor = System.Drawing.Color.Transparent;

            foreach (var tb in new System.Windows.Forms.TextBox[] { textBox1, textBox2, textBox3, textBox4 })
            {
                tb.BackColor = System.Drawing.Color.FromArgb(235, 235, 238);
                tb.ForeColor = System.Drawing.Color.FromArgb(40, 40, 52);
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
            textBox3.PasswordChar = '●';
            textBox4.PasswordChar = '●';

            button1.BackColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button1.ForeColor = System.Drawing.Color.White;
            button1.FlatStyle = FlatStyle.Flat;
            button1.FlatAppearance.BorderSize = 0;
            button1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold);
            button1.Cursor = Cursors.Hand;
            button1.UseVisualStyleBackColor = false;
            button2.BackColor = System.Drawing.Color.Transparent;
            button2.ForeColor = System.Drawing.Color.FromArgb(242, 159, 103);
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.Font = new System.Drawing.Font("Segoe UI", 10.2F);
            button2.Cursor = Cursors.Hand;
            button2.UseVisualStyleBackColor = false;
        }
        private void REGISTRATION_Load(object sender, EventArgs e)
        {
            ApplyTheme();
        }

        private void button1_Click(object sender, EventArgs e) //create acccc
        {
            string username = textBox1.Text.Trim();
            string email = textBox2.Text.Trim();
            string password = textBox3.Text;
            string confirmPassword = textBox4.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Invalid Email",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.", "Weak Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Password Mismatch",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Clear(); textBox4.Clear(); textBox3.Focus();
                return;
            }
            try
            {
                if (DatabaseHelper.EmailExists(email))
                {
                    MessageBox.Show("An account with that email already exists.",
                        "Email Taken", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (DatabaseHelper.RegisterUser(username, email, password))
                {
                    MessageBox.Show($"Account created! Welcome, {username}.",
                        "Registration Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LOGIN login = new LOGIN();
                    login.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Registration failed. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e) //back to login
        {
            LOGIN login = new LOGIN();
            login.Show();
            this.Close();
        }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
    }
}