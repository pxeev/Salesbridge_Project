using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace Salesbridge
{
    internal class OrderItem
    {
        public int ProductId;
        public string Name;
        public decimal Price;
        public int Qty;
    }
    public partial class Form1 : Form
    {
        private readonly List<OrderItem> _order = new List<OrderItem>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            groupBox6.Visible = false;
            WireOrderButtons();
            WireNavButtons();
            LoadProductCards();
        }

        private FlowLayoutPanel _productFlow;
        private void LoadProductCards()
        {
            if (_productFlow != null)
            {
                this.Controls.Remove(_productFlow);
                _productFlow.Dispose();
            }
            _productFlow = new FlowLayoutPanel();
            _productFlow.Location = new Point(231, 65);
            _productFlow.Size = new Size(530, 300);
            _productFlow.AutoScroll = true;
            _productFlow.FlowDirection = FlowDirection.LeftToRight;
            _productFlow.WrapContents = true;
            _productFlow.BackColor = Color.Transparent;
            _productFlow.Padding = new Padding(2);
            this.Controls.Add(_productFlow);

            try
            {
                DataTable products = DatabaseHelper.GetProductsForPOS();

                if (products.Rows.Count == 0)
                {
                    var noItem = new Label();
                    noItem.Text = "No products available.\nAdd products in Inventory.";
                    noItem.AutoSize = true;
                    noItem.Font = new Font("Microsoft Sans Serif", 9F);
                    _productFlow.Controls.Add(noItem);
                    return;
                }

                foreach (DataRow row in products.Rows)
                {
                    int id = Convert.ToInt32(row["Id"]);
                    string name = row["ProductName"].ToString();
                    decimal price = Convert.ToDecimal(row["Price"]);
                    int stock = Convert.ToInt32(row["Stock"]);

                    var card = new GroupBox();
                    card.Text = name;
                    card.Size = new Size(158, 130);
                    card.Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Bold);
                    card.Margin = new Padding(4);
                    card.BackColor = Color.White;

                    var priceLabel = new Label();
                    priceLabel.Text = $"₱{price:N2}";
                    priceLabel.Location = new Point(35, 55);
                    priceLabel.AutoSize = true;
                    priceLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
                    priceLabel.ForeColor = Color.Black;

                    var stockLabel = new Label();
                    stockLabel.Text = $"Stock: {stock}";
                    stockLabel.Location = new Point(35, 75);
                    stockLabel.AutoSize = true;
                    stockLabel.Font = new Font("Microsoft Sans Serif", 7F);
                    stockLabel.ForeColor = stock <= 5 ? Color.Red :
                                          stock <= 15 ? Color.DarkOrange : Color.Gray;

                    var addBtn = new Button();
                    addBtn.Text = "Add";
                    addBtn.Location = new Point(40, 97);
                    addBtn.Size = new Size(74, 24);
                    addBtn.Font = new Font("Microsoft Sans Serif", 7.8F, FontStyle.Bold);
                    addBtn.Enabled = stock > 0; // disable if out of stock

                    int capturedId = id;
                    string capturedName = name;
                    decimal capturedPrice = price;

                    addBtn.Click += (s, ev) =>
                        AddToOrder(capturedId, capturedName, capturedPrice);

                    card.Controls.Add(priceLabel);
                    card.Controls.Add(stockLabel);
                    card.Controls.Add(addBtn);
                    _productFlow.Controls.Add(card);
                }
            }
            catch (Exception ex)
            {
                var errLabel = new Label();
                errLabel.Text = "Error loading products:\n" + ex.Message;
                errLabel.AutoSize = true;
                errLabel.ForeColor = Color.Red;
                _productFlow.Controls.Add(errLabel);
            }
        }

        private void AddToOrder(int productId, string name, decimal price)
        {
            OrderItem existing = _order.Find(o => o.ProductId == productId);
            if (existing != null) existing.Qty++;
            else _order.Add(new OrderItem
            { ProductId = productId, Name = name, Price = price, Qty = 1 });

            RefreshOrderDisplay();
        }
        private void RefreshOrderDisplay()
        {
            richTextBox2.Clear();
            decimal total = 0;
            foreach (var item in _order)
            {
                decimal line = item.Price * item.Qty;
                total += line;
                richTextBox2.AppendText($"{item.Name} x{item.Qty} = ₱{line:N2}\n");
            }
            richTextBox2.AppendText($"\nTOTAL: ₱{total:N2}");
        }

        private void WireOrderButtons()
        {
            button14.Click += (s, e) =>
            {
                _order.Clear();
                richTextBox2.Clear();
            };

            button15.Click += (s, e) => ProcessPayment();
        }

        private void ProcessPayment()
        {
            if (_order.Count == 0)
            {
                MessageBox.Show("No items in the current order.", "Empty Order",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = 0;
            var itemList = new System.Text.StringBuilder();
            foreach (var item in _order)
            {
                total += item.Price * item.Qty;
                if (itemList.Length > 0) itemList.Append(", ");
                itemList.Append($"{item.Name} x{item.Qty}");
            }

            if (MessageBox.Show(
                $"Process payment of ₱{total:N2}?\n\nItems: {itemList}",
                "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                int txId = DatabaseHelper.AddTransaction(
                    itemList.ToString(), total, AppSession.CurrentUsername);

                foreach (var item in _order)
                    DatabaseHelper.DeductStockAndUpdateSales(
                        item.ProductId, item.Qty, item.Price);

                AppSession.RaiseNotification(
                    $"POS Payment by {AppSession.CurrentUsername} — " +
                    $"{itemList} | ₱{total:N2}");

                MessageBox.Show($"Payment successful! Transaction ID: {txId}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _order.Clear();
                richTextBox2.Clear();

                // Refresh product cards to reflect updated stock levels
                LoadProductCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing payment:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Navigate(string module)
        {
            foreach (Form f in Application.OpenForms)
                if (f is DASHBOARD dash) { dash.NavigateTo(module); return; }
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
        private void button1_Click(object sender, EventArgs e) { /* now handled dynamically */ }
    }
}