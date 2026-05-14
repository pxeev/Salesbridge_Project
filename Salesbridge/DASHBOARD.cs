using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salesbridge
{
    public partial class DASHBOARD : Form
    {
        private DataTable _revenueTable = new DataTable();
        private DataTable _transactionTable = new DataTable();
        private DataTable _inventoryTable = new DataTable();

        public DASHBOARD()
        {
            InitializeComponent();
        }

        private void DASHBOARD_Load(object sender, EventArgs e)
        {
            try
            {
                SetupGridColumns();
                InitializeDashboardTables();
                LoadDashboardData();
                textBox1.TextChanged += textBox1_TextChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error loading dashboard:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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
            dataGridViewTextBoxColumn3.DataPropertyName = "TIME";
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

        private void InitializeDashboardTables()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.AppConnection))
            {
                conn.Open();

                string createProducts =
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U') " +
                    "CREATE TABLE Products (" +
                    "    Id          INT IDENTITY(1,1) PRIMARY KEY, " +
                    "    ProductName NVARCHAR(200) NOT NULL, " +
                    "    Category    NVARCHAR(100), " +
                    "    Price       DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                    "    Stock       INT           NOT NULL DEFAULT 0, " +
                    "    Status      NVARCHAR(50)  NOT NULL DEFAULT 'Available', " +
                    "    Revenue     DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                    "    UnitsSold   INT           NOT NULL DEFAULT 0);";
                new SqlCommand(createProducts, conn).ExecuteNonQuery();

                string createTransactions =
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U') " +
                    "CREATE TABLE Transactions (" +
                    "    Id      INT IDENTITY(1,1) PRIMARY KEY, " +
                    "    TxDate  DATE          NOT NULL DEFAULT GETDATE(), " +
                    "    TxTime  TIME          NOT NULL DEFAULT CONVERT(TIME,GETDATE()), " +
                    "    Total   DECIMAL(10,2) NOT NULL DEFAULT 0, " +
                    "    Cashier NVARCHAR(100), " +
                    "    Status  NVARCHAR(50)  NOT NULL DEFAULT 'Completed');";
                new SqlCommand(createTransactions, conn).ExecuteNonQuery();
            }
        }

        private void LoadDashboardData()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseHelper.AppConnection))
            {
                conn.Open();

                string revenueSQL =
                    "SELECT Id          AS [ID], " +
                    "       ProductName AS [Product], " +
                    "       Revenue     AS [Revenue], " +
                    "       UnitsSold   AS [Units Sold] " +
                    "FROM   Products " +
                    "ORDER  BY Revenue DESC;";
                SqlDataAdapter daRev = new SqlDataAdapter(revenueSQL, conn);
                _revenueTable = new DataTable();
                daRev.Fill(_revenueTable);
                dataGridView1.DataSource = _revenueTable;

                string txSQL =
                    "SELECT TOP 50 " +
                    "    Id                                   AS [ID], " +
                    "    CONVERT(NVARCHAR,TxDate,101)         AS [DATE], " +
                    "    CONVERT(NVARCHAR,TxTime,100)         AS [TIME], " +
                    "    Total                                AS [TOTAL], " +
                    "    Cashier                              AS [CASHIER], " +
                    "    Status                               AS [STATUS] " +
                    "FROM  Transactions " +
                    "ORDER BY Id DESC;";
                SqlDataAdapter daTx = new SqlDataAdapter(txSQL, conn);
                _transactionTable = new DataTable();
                daTx.Fill(_transactionTable);
                dataGridView2.DataSource = _transactionTable;

                string invSQL =
                    "SELECT ProductName AS [PRODUCT], " +
                    "       Category    AS [CATEGORY], " +
                    "       Price       AS [PRICE], " +
                    "       Stock       AS [STOCK], " +
                    "       Status      AS [STATUS] " +
                    "FROM   Products " +
                    "ORDER  BY ProductName;";
                SqlDataAdapter daInv = new SqlDataAdapter(invSQL, conn);
                _inventoryTable = new DataTable();
                daInv.Fill(_inventoryTable);
                dataGridView3.DataSource = _inventoryTable;
            }
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
                DataView dvRev = new DataView(_revenueTable);
                dvRev.RowFilter =
                    $"Convert([ID],'System.String') LIKE '%{kw}%' " +
                    $"OR [Product] LIKE '%{kw}%'";
                dataGridView1.DataSource = dvRev;

                DataView dvTx = new DataView(_transactionTable);
                dvTx.RowFilter =
                    $"Convert([ID],'System.String') LIKE '%{kw}%' " +
                    $"OR [CASHIER] LIKE '%{kw}%' " +
                    $"OR [STATUS]  LIKE '%{kw}%' " +
                    $"OR [DATE]    LIKE '%{kw}%'";
                dataGridView2.DataSource = dvTx;

                DataView dvInv = new DataView(_inventoryTable);
                dvInv.RowFilter =
                    $"[PRODUCT]  LIKE '%{kw}%' " +
                    $"OR [CATEGORY] LIKE '%{kw}%' " +
                    $"OR [STATUS]   LIKE '%{kw}%'";
                dataGridView3.DataSource = dvInv;
            }
            catch
            {
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Transaction module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("POS module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Inventory module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("No new notifications.", "Notifications",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            button5_Click(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Analytics module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            button6_Click(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                LOGIN loginForm = new LOGIN();
                loginForm.Show();
                this.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Clear();
            try { LoadDashboardData(); }
            catch (Exception ex)
            {
                MessageBox.Show("Refresh error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Transaction module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("POS module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Inventory module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void button5_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("No new notifications.", "Notifications",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Analytics module coming soon.", "Salesbridge",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                LOGIN loginForm = new LOGIN();
                loginForm.Show();
                this.Close();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}