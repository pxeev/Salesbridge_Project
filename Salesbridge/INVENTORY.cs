using System;
using System.Data;
using System.Windows.Forms;

namespace Salesbridge
{
    public partial class INVENTORY : Form
    {
        private DataTable _productTable = new DataTable();

        public INVENTORY()
        {
            InitializeComponent();
        }

        private void INVENTORY_Load(object sender, EventArgs e)
        {
            Column1.DataPropertyName = "PRODUCT";
            Column2.DataPropertyName = "CATEGORY";
            Column3.DataPropertyName = "PRICE";
            Column4.DataPropertyName = "STOCK";
            Column5.DataPropertyName = "STATUS";
            dataGridView1.AutoGenerateColumns = false;

            WireNavButtons();
            button1.Click += button1_AddProduct_Click;
            LoadInventory();
        }

        private void LoadInventory()
        {
            try
            {
                _productTable = DatabaseHelper.GetProducts();
                dataGridView1.DataSource = _productTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_AddProduct_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Product name:", "Add Product", "");
            if (string.IsNullOrWhiteSpace(name)) return;

            string category = Microsoft.VisualBasic.Interaction.InputBox(
                "Category:", "Add Product", "Beverages");
            if (string.IsNullOrWhiteSpace(category)) category = "General";

            string priceStr = Microsoft.VisualBasic.Interaction.InputBox(
                "Price:", "Add Product", "0");
            if (!decimal.TryParse(priceStr, out decimal price) || price < 0)
            {
                MessageBox.Show("Invalid price.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string stockStr = Microsoft.VisualBasic.Interaction.InputBox(
                "Initial stock quantity:", "Add Product", "0");
            if (!int.TryParse(stockStr, out int stock) || stock < 0)
            {
                MessageBox.Show("Invalid stock amount.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DatabaseHelper.AddProduct(name, category, price, stock);
                AppSession.RaiseNotification(
                    $"{AppSession.CurrentUsername} added product: {name} (Stock: {stock})");
                LoadInventory();
                MessageBox.Show($"Product \"{name}\" added.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string kw = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(kw))
            {
                dataGridView1.DataSource = _productTable;
                return;
            }
            try
            {
                var dv = new DataView(_productTable);
                dv.RowFilter = $"[PRODUCT] LIKE '%{kw}%' OR [CATEGORY] LIKE '%{kw}%' OR [STATUS] LIKE '%{kw}%'";
                dataGridView1.DataSource = dv;
            }
            catch { }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _productTable.Rows.Count) return;
            if (!int.TryParse(_productTable.Rows[e.RowIndex]["ID"].ToString(), out int prodId)) return;

            string prodName = _productTable.Rows[e.RowIndex]["PRODUCT"].ToString();
            string currStock = _productTable.Rows[e.RowIndex]["STOCK"].ToString();

            string newStockStr = Microsoft.VisualBasic.Interaction.InputBox(
                $"Update stock for \"{prodName}\" (current: {currStock}):",
                "Update Stock", currStock);

            if (!int.TryParse(newStockStr, out int newStock) || newStock < 0) return;

            try
            {
                DatabaseHelper.UpdateProductStock(prodId, newStock);
                AppSession.RaiseNotification(
                    $"{AppSession.CurrentUsername} updated stock: {prodName} {currStock}→{newStock}");
                LoadInventory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock:\n" + ex.Message, "Error",
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
        private void pictureBox2_Click(object sender, EventArgs e) { }
        private void pictureBox3_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }
        private void richTextBox2_TextChanged(object sender, EventArgs e) { }
    }
}