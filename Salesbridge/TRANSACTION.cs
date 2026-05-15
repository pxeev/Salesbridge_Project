using System;
using System.Data;
using System.Windows.Forms;
namespace Salesbridge
{
    public partial class TRANSACTION : Form
    {
        private DataTable _txTable = new DataTable();

        public TRANSACTION()
        {
            InitializeComponent();
            this.Load += TRANSACTION_Load;
        }
        private void TRANSACTION_Load(object sender, EventArgs e)
        {
            Column3.HeaderText = "ITEMS";
            Column1.DataPropertyName = "ID";
            Column2.DataPropertyName = "DATE";
            Column3.DataPropertyName = "ITEMS";
            Column4.DataPropertyName = "TOTAL";
            Column5.DataPropertyName = "CASHIER";
            Column6.DataPropertyName = "STATUS";
            dataGridView1.AutoGenerateColumns = false;
            WireNavButtons();
            SetupAddRecord();
            LoadTransactions();
        }

        private void SetupAddRecord()
        {
            foreach (Control c in this.Controls)
            {
                switch (c.Name)
                {
                    case "textBox1":
                        c.Visible = false;
                        break;
                    case "pictureBox2":
                        c.Cursor = Cursors.Hand;
                        c.Click += (s, ev) => ShowAddRecordDialog();
                        break;
                }
            }
        }

        private void LoadTransactions()
        {
            try
            {
                _txTable = DatabaseHelper.GetTransactions();
                dataGridView1.DataSource = _txTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading transactions:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAddRecordDialog()
        {
            string items = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter items (e.g. Espresso x1, Latte x2):", "Add Record - Items", "");
            if (string.IsNullOrWhiteSpace(items)) return;

            string totalStr = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter total amount:", "Add Record - Total", "0");
            if (!decimal.TryParse(totalStr, out decimal total) || total < 0)
            {
                MessageBox.Show("Invalid total amount.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cashier = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter cashier name:", "Add Record - Cashier", AppSession.CurrentUsername);
            if (string.IsNullOrWhiteSpace(cashier)) cashier = AppSession.CurrentUsername;

            try
            {
                DatabaseHelper.AddTransaction(items, total, cashier);
                AppSession.RaiseNotification(
                    $"Manual transaction added by {cashier} — {items} (₱{total:N2})");
                LoadTransactions();
                MessageBox.Show("Transaction record added.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving transaction:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Navigate(string module)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is DASHBOARD dash) { dash.NavigateTo(module); return; }
            }
            if (module == "Logout") { new LOGIN().Show(); this.ParentForm?.Close(); }
        }

        private void WireNavButtons()
        {
            button8.Click += (s, ev) => Navigate("Dashboard");
            button2.Click += (s, ev) => Navigate("Transaction");
            button3.Click += (s, ev) => Navigate("POS");
            button4.Click += (s, ev) => Navigate("Inventory");
            button5.Click += (s, ev) => Navigate("Notification");
            button6.Click += (s, ev) => Navigate("Analytics");
            button7.Click += (s, ev) => Navigate("Logout");
        }
        private void richTextBox4_TextChanged(object sender, EventArgs e) { }
        private void richTextBox5_TextChanged(object sender, EventArgs e) { }
    }
}