using System;
using System.Data;
using System.Windows.Forms;
namespace Salesbridge
{
    public partial class NOTIFICATION : Form
    {
        public NOTIFICATION()
        {
            InitializeComponent();
            this.Load += NOTIFICATION_Load;
        }
        private void NOTIFICATION_Load(object sender, EventArgs e)
        {
            WireNavButtons();
            listBox1.Items.Clear();
            LoadNotifications();
            AppSession.NotificationAdded += OnNewNotification; //live notifierrr
        }

        private void OnNewNotification(string message)
        {
            if (this.InvokeRequired) this.Invoke(new Action(LoadNotifications));
            else LoadNotifications();
        }

        private void LoadNotifications()
        {
            listBox1.Items.Clear();
            try
            {
                DataTable dt = DatabaseHelper.GetNotifications();
                foreach (DataRow row in dt.Rows)
                    listBox1.Items.Add($"[{row["CreatedAt"]}]  {row["Message"]}");

                if (listBox1.Items.Count == 0)
                    listBox1.Items.Add("No notifications yet. Actions will appear here.");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Error loading notifications: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) //marker as reads nga ni
        {
            try
            {
                DatabaseHelper.MarkAllNotificationsRead();
                listBox1.Items.Clear();
                listBox1.Items.Add("All notifications marked as read.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            AppSession.NotificationAdded -= OnNewNotification;
            base.OnFormClosed(e);
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
            button4.Click += (s, ev) => Navigate("Notification");
            button5.Click += (s, ev) => Navigate("Notification");
            button6.Click += (s, ev) => Navigate("Analytics");
            button7.Click += (s, ev) => Navigate("Logout");
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}