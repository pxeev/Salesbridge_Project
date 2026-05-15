using System;
using System.Data;
using System.Windows.Forms;

namespace Salesbridge
{
    public partial class ANALYTICS : Form
    {
        public ANALYTICS()
        {
            InitializeComponent();
        }

        private void ANALYTICS_Load(object sender, EventArgs e)
        {
            Column4.DataPropertyName = "ID";
            Column1.DataPropertyName = "Product";
            Column2.DataPropertyName = "Revenue";
            Column3.DataPropertyName = "Units Sold";
            dataGridView1.AutoGenerateColumns = false;

            WireNavButtons();
            LoadAnalytics();
        }

        private void LoadAnalytics()
        {
            try
            {
                DataTable dt = DatabaseHelper.GetAnalytics();
                dataGridView1.DataSource = dt;

                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                    total += Convert.ToDecimal(row["Revenue"]);

                label3.Text = $"Total Revenue: ₱{total:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading analytics:\n" + ex.Message, "Error",
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
            button1.Click += (s, ev) => Navigate("Dashboard");
            button2.Click += (s, ev) => Navigate("Transaction");
            button3.Click += (s, ev) => Navigate("POS");
            button4.Click += (s, ev) => Navigate("Inventory");
            button5.Click += (s, ev) => Navigate("Notification");
            button6.Click += (s, ev) => Navigate("Analytics");
            button7.Click += (s, ev) => Navigate("Logout");
        }
        private void chart1_Click(object sender, EventArgs e) { }
        private void chart1_Click_1(object sender, EventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}