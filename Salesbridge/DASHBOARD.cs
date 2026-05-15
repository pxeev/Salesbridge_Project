using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Salesbridge
{
    public partial class DASHBOARD : Form
    {
        private DataTable _revenueTable = new DataTable();
        private DataTable _transactionTable = new DataTable();
        private DataTable _inventoryTable = new DataTable();
        private Panel _contentPanel;

        public DASHBOARD()
        {
            InitializeComponent();
            CreateContentPanel();
        }
        private void CreateContentPanel()
        {
            _contentPanel = new Panel();
            _contentPanel.Left = 354;
            _contentPanel.Top = 0;
            _contentPanel.Width = Math.Max(100, this.ClientSize.Width - 354);
            _contentPanel.Height = this.ClientSize.Height;
            _contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                      AnchorStyles.Left | AnchorStyles.Right;
            _contentPanel.BackColor = Color.WhiteSmoke;
            _contentPanel.Visible = false;
            this.Controls.Add(_contentPanel);
            this.Controls.SetChildIndex(_contentPanel, this.Controls.Count - 1);
        }

        private void DASHBOARD_Load(object sender, EventArgs e)
        {
            label1.Text = $"SalesBridge  |  {AppSession.CurrentUsername}";
            SetupGridColumns();
            LoadDashboardData();
            textBox1.TextChanged += textBox1_TextChanged;
        }
        private void SetupGridColumns()
        {
            dataGridView1.AutoGenerateColumns = false;
            Column4.DataPropertyName = "ID";
            Column1.DataPropertyName = "Product";
            Column2.DataPropertyName = "Revenue";
            Column3.DataPropertyName = "Units Sold";
            dataGridView2.AutoGenerateColumns = false;
            dataGridViewTextBoxColumn1.DataPropertyName = "ID";
            dataGridViewTextBoxColumn2.DataPropertyName = "DATE";
            dataGridViewTextBoxColumn3.DataPropertyName = "ITEMS";
            dataGridViewTextBoxColumn3.HeaderText = "ITEMS";
            dataGridViewTextBoxColumn4.DataPropertyName = "TOTAL";
            dataGridViewTextBoxColumn5.DataPropertyName = "CASHIER";
            dataGridViewTextBoxColumn6.DataPropertyName = "STATUS";
            dataGridView3.AutoGenerateColumns = false;
            dataGridViewTextBoxColumn7.DataPropertyName = "PRODUCT";
            dataGridViewTextBoxColumn8.DataPropertyName = "CATEGORY";
            dataGridViewTextBoxColumn9.DataPropertyName = "PRICE";
            dataGridViewTextBoxColumn10.DataPropertyName = "STOCK";
            dataGridViewTextBoxColumn11.DataPropertyName = "STATUS";
        }
        private void LoadDashboardData()
        {
            try
            {
                _revenueTable = DatabaseHelper.GetRevenueTable();
                dataGridView1.DataSource = _revenueTable;

                _transactionTable = DatabaseHelper.GetRecentTransactions();
                dataGridView2.DataSource = _transactionTable;

                _inventoryTable = DatabaseHelper.GetInventoryTable();
                dataGridView3.DataSource = _inventoryTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void NavigateTo(string module)
        {
            switch (module)
            {
                case "Dashboard": ShowDashboardSummary(); break;
                case "Transaction": EmbedChild(new TRANSACTION()); break;
                case "POS": EmbedChild(new Form1()); break;
                case "Inventory": EmbedChild(new INVENTORY()); break;
                case "Notification": EmbedChild(new NOTIFICATION()); break;
                case "Analytics": EmbedChild(new ANALYTICS()); break;
                case "Logout": Logout(); break;
            }
        }
        private void EmbedChild(Form child)
        {
            foreach (Control c in _contentPanel.Controls)
                if (c is Form f) f.Close();
            _contentPanel.Controls.Clear();

            SetDashboardSummaryVisible(false);
            child.TopLevel = false;
            child.FormBorderStyle = FormBorderStyle.None;
            child.Location = new Point(0, 0);
            child.Size = _contentPanel.ClientSize;
            _contentPanel.Controls.Add(child);
            _contentPanel.Visible = true;
            child.Show();
            this.BeginInvoke(new Action(() =>
            {
                HideChildSidebar(child);
                ShiftChildContent(child);
            }));
        }

        private static readonly HashSet<string> _sidebarNames = new HashSet<string>
        {
            "richTextBox1", "label1", "pictureBox1",
            "pictureBox3",  "pictureBox4",  "pictureBox5",
            "pictureBox6",  "pictureBox7",  "pictureBox8", "pictureBox9"
        };

        private void HideChildSidebar(Form child)
        {
            foreach (Control c in child.Controls)
            {
                if (_sidebarNames.Contains(c.Name))
                {
                    c.Visible = false;
                    continue;
                }
                if (c is Button && c.Left < 200)
                    c.Visible = false;
            }
        }

        private void ShiftChildContent(Form child)
        {
            int minX = int.MaxValue;
            foreach (Control c in child.Controls)
                if (c.Visible && c.Left < minX)
                    minX = c.Left;

            if (minX == int.MaxValue || minX <= 10) return;
            int shift = minX - 5;
            foreach (Control c in child.Controls)
                if (c.Visible)
                    c.Left = Math.Max(0, c.Left - shift);
        }
        private void ShowDashboardSummary()
        {
            foreach (Control c in _contentPanel.Controls)
                if (c is Form f) f.Close();
            _contentPanel.Controls.Clear();
            _contentPanel.Visible = false;
            SetDashboardSummaryVisible(true);
            textBox1.Clear();
            try { LoadDashboardData(); } catch { }
        }

        private void SetDashboardSummaryVisible(bool visible)
        {
            dataGridView1.Visible = visible;
            dataGridView2.Visible = visible;
            dataGridView3.Visible = visible;
            label2.Visible = visible;
            label3.Visible = visible;
            label4.Visible = visible;
            label8.Visible = visible;
            textBox1.Visible = visible;
            pictureBox2.Visible = visible;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string kw = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(kw))
            {
                dataGridView1.DataSource = _revenueTable;
                dataGridView2.DataSource = _transactionTable;
                dataGridView3.DataSource = _inventoryTable;
                return;
            }
            try
            {
                var dvRev = new DataView(_revenueTable);
                dvRev.RowFilter = $"[Product] LIKE '%{kw}%'";
                dataGridView1.DataSource = dvRev;

                var dvTx = new DataView(_transactionTable);
                dvTx.RowFilter = $"[CASHIER] LIKE '%{kw}%' OR [STATUS] LIKE '%{kw}%' OR [DATE] LIKE '%{kw}%'";
                dataGridView2.DataSource = dvTx;

                var dvInv = new DataView(_inventoryTable);
                dvInv.RowFilter = $"[PRODUCT] LIKE '%{kw}%' OR [CATEGORY] LIKE '%{kw}%' OR [STATUS] LIKE '%{kw}%'";
                dataGridView3.DataSource = dvInv;
            }
            catch { }
        }

        private void button1_Click_1(object sender, EventArgs e) => ShowDashboardSummary();
        private void button2_Click_1(object sender, EventArgs e) => EmbedChild(new TRANSACTION());
        private void button3_Click_1(object sender, EventArgs e) => EmbedChild(new Form1());
        private void button4_Click_1(object sender, EventArgs e) => EmbedChild(new INVENTORY());
        private void button5_Click_1(object sender, EventArgs e) => EmbedChild(new NOTIFICATION());
        private void button6_Click_1(object sender, EventArgs e) => EmbedChild(new ANALYTICS());
        private void button7_Click_1(object sender, EventArgs e) => Logout();
        private void pictureBox8_Click(object sender, EventArgs e) => EmbedChild(new NOTIFICATION());
        private void pictureBox9_Click(object sender, EventArgs e) => EmbedChild(new ANALYTICS());
        private void pictureBox3_Click(object sender, EventArgs e) => Logout();
        public void Logout()
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                new LOGIN().Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e) => EmbedChild(new TRANSACTION());
        private void button3_Click(object sender, EventArgs e) => EmbedChild(new Form1());
        private void button4_Click(object sender, EventArgs e) => EmbedChild(new INVENTORY());
        private void button5_Click(object sender, EventArgs e) => EmbedChild(new NOTIFICATION());
        private void button6_Click(object sender, EventArgs e) => EmbedChild(new ANALYTICS());
        private void button7_Click(object sender, EventArgs e) => Logout();
        private void pictureBox4_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
    }
}