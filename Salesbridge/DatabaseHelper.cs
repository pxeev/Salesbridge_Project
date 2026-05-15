using System;
using System.Data;
using System.Data.SqlClient;
namespace Salesbridge
{
    public static class DatabaseHelper
    {
        private static readonly string MasterConnection =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
        public static readonly string AppConnection =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SalesbridgeDB;Integrated Security=True;";

        public static void InitializeDatabase() //init
        {
            using (SqlConnection conn = new SqlConnection(MasterConnection))
            {
                conn.Open();
                string sql =
                    "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name=N'SalesbridgeDB') " +
                    "CREATE DATABASE SalesbridgeDB;";
                new SqlCommand(sql, conn).ExecuteNonQuery();
            }

            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();

                new SqlCommand( //user dbs
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U') " +
                    "CREATE TABLE Users (Id INT IDENTITY(1,1) PRIMARY KEY, " +
                    "Username NVARCHAR(100) NOT NULL, Email NVARCHAR(200) NOT NULL UNIQUE, " +
                    "Password NVARCHAR(200) NOT NULL);", conn).ExecuteNonQuery();

                new SqlCommand( //prod dbs
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Products' AND xtype='U') " +
                    "CREATE TABLE Products (Id INT IDENTITY(1,1) PRIMARY KEY, " +
                    "ProductName NVARCHAR(200) NOT NULL, Category NVARCHAR(100), " +
                    "Price DECIMAL(10,2) NOT NULL DEFAULT 0, Stock INT NOT NULL DEFAULT 0, " +
                    "Status NVARCHAR(50) NOT NULL DEFAULT 'Available', " +
                    "Revenue DECIMAL(10,2) NOT NULL DEFAULT 0, UnitsSold INT NOT NULL DEFAULT 0);",
                    conn).ExecuteNonQuery();

                new SqlCommand( //transaksyon dbs
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U') " +
                    "CREATE TABLE Transactions (Id INT IDENTITY(1,1) PRIMARY KEY, " +
                    "TxDate DATE NOT NULL DEFAULT GETDATE(), Items NVARCHAR(500) NOT NULL DEFAULT '', " +
                    "Total DECIMAL(10,2) NOT NULL DEFAULT 0, Cashier NVARCHAR(100), " +
                    "Status NVARCHAR(50) NOT NULL DEFAULT 'Completed');", conn).ExecuteNonQuery();

                new SqlCommand( //notifs dbs
                    "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U') " +
                    "CREATE TABLE Notifications (Id INT IDENTITY(1,1) PRIMARY KEY, " +
                    "Message NVARCHAR(500) NOT NULL, CreatedAt DATETIME NOT NULL DEFAULT GETDATE(), " +
                    "IsRead BIT NOT NULL DEFAULT 0);", conn).ExecuteNonQuery();
                SeedDummyProducts(conn);
            }
        }

        private static void SeedDummyProducts(SqlConnection conn)
        {
            int count = (int)new SqlCommand("SELECT COUNT(*) FROM Products;", conn).ExecuteScalar();
            if (count > 0) return;

            string seed =
                "INSERT INTO Products (ProductName,Category,Price,Stock,Status) VALUES " +
                "('Espresso','Beverages',120,50,'Available')," +
                "('Cappuccino','Beverages',150,45,'Available')," +
                "('Latte','Beverages',160,40,'Available')," +
                "('Americano','Beverages',130,55,'Available')," +
                "('Green Tea','Beverages',110,30,'Available')," +
                "('Hot Chocolate','Beverages',140,35,'Available');";
            new SqlCommand(seed, conn).ExecuteNonQuery();
        }

        public static bool ValidateUser(string email, string password) //user validasyon
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email=@E AND Password=@P;", conn);
                cmd.Parameters.AddWithValue("@E", email.Trim());
                cmd.Parameters.AddWithValue("@P", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public static bool EmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email=@E;", conn);
                cmd.Parameters.AddWithValue("@E", email.Trim());
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public static bool RegisterUser(string username, string email, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConnection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Users (Username,Email,Password) VALUES (@U,@E,@P);", conn);
                    cmd.Parameters.AddWithValue("@U", username.Trim());
                    cmd.Parameters.AddWithValue("@E", email.Trim());
                    cmd.Parameters.AddWithValue("@P", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException) { return false; }
        }
        public static string GetUsernameByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT Username FROM Users WHERE Email=@E;", conn);
                cmd.Parameters.AddWithValue("@E", email.Trim());
                return cmd.ExecuteScalar()?.ToString();
            }
        }


        public static DataTable GetProducts() //prods and inv dbs
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT Id AS [ID], ProductName AS [PRODUCT], Category AS [CATEGORY], " +
                    "Price AS [PRICE], Stock AS [STOCK], " +
                    "CASE WHEN Stock<=5 THEN 'Critical' WHEN Stock<=15 THEN 'Low Stock' " +
                    "     ELSE 'Available' END AS [STATUS] " +
                    "FROM Products ORDER BY ProductName;";
                DataTable dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                return dt;
            }
        }

        public static DataTable GetProductsForPOS()
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT Id, ProductName, Price, Stock FROM Products ORDER BY Id;", conn)
                    .Fill(dt);
                return dt;
            }
        }

        public static void AddProduct(string name, string category, decimal price, int stock)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Products (ProductName,Category,Price,Stock,Status) " +
                    "VALUES (@N,@C,@P,@S,'Available');", conn);
                cmd.Parameters.AddWithValue("@N", name.Trim());
                cmd.Parameters.AddWithValue("@C", category.Trim());
                cmd.Parameters.AddWithValue("@P", price);
                cmd.Parameters.AddWithValue("@S", stock);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateProductStock(int id, int newStock)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Products SET Stock=@S WHERE Id=@Id;", conn);
                cmd.Parameters.AddWithValue("@S", newStock);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public static void DeductStockAndUpdateSales(int productId, int qty, decimal unitPrice) //deducts product if pos system is use
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Products SET Stock = Stock - @Qty, " +
                    "UnitsSold = UnitsSold + @Qty, Revenue = Revenue + @Rev " +
                    "WHERE Id = @Id AND Stock >= @Qty;", conn);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@Rev", qty * unitPrice);
                cmd.Parameters.AddWithValue("@Id", productId);
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetTransactions() //transacs dbs
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT TOP 200 Id AS [ID], " +
                    "CONVERT(NVARCHAR,TxDate,101) AS [DATE], " +
                    "Items AS [ITEMS], Total AS [TOTAL], " +
                    "Cashier AS [CASHIER], Status AS [STATUS] " +
                    "FROM Transactions ORDER BY Id DESC;";
                DataTable dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                return dt;
            }
        }

        public static DataTable GetRecentTransactions()
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT TOP 50 Id AS [ID], " +
                    "CONVERT(NVARCHAR,TxDate,101) AS [DATE], " +
                    "Items AS [ITEMS], Total AS [TOTAL], " +
                    "Cashier AS [CASHIER], Status AS [STATUS] " +
                    "FROM Transactions ORDER BY Id DESC;";
                DataTable dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                return dt;
            }
        }

        public static int AddTransaction(string items, decimal total, string cashier)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Transactions (Items,Total,Cashier,Status) " +
                    "OUTPUT INSERTED.Id VALUES (@I,@T,@C,'Completed');", conn);
                cmd.Parameters.AddWithValue("@I", items);
                cmd.Parameters.AddWithValue("@T", total);
                cmd.Parameters.AddWithValue("@C", cashier);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static DataTable GetAnalytics() //analytics dbs
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT Id AS [ID], ProductName AS [Product], " +
                    "Revenue AS [Revenue], UnitsSold AS [Units Sold] " +
                    "FROM Products ORDER BY Revenue DESC;";
                DataTable dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                return dt;
            }
        }

        public static DataTable GetNotifications() //notifs dbsss
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT TOP 100 Message, CONVERT(NVARCHAR,CreatedAt,120) AS CreatedAt " +
                    "FROM Notifications ORDER BY Id DESC;", conn).Fill(dt);
                return dt;
            }
        }

        public static void AddNotification(string message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppConnection))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Notifications (Message) VALUES (@M);", conn);
                    cmd.Parameters.AddWithValue("@M", message);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* non-critical */ }
        }

        public static void MarkAllNotificationsRead()
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                new SqlCommand("UPDATE Notifications SET IsRead=1;", conn).ExecuteNonQuery();
            }
        }


        public static DataTable GetRevenueTable() //dashburd sum helper
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(
                    "SELECT Id AS [ID], ProductName AS [Product], " +
                    "Revenue AS [Revenue], UnitsSold AS [Units Sold] " +
                    "FROM Products ORDER BY Revenue DESC;", conn).Fill(dt);
                return dt;
            }
        }

        public static DataTable GetInventoryTable()
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT ProductName AS [PRODUCT], Category AS [CATEGORY], " +
                    "Price AS [PRICE], Stock AS [STOCK], " +
                    "CASE WHEN Stock<=5 THEN 'Critical' WHEN Stock<=15 THEN 'Low Stock' " +
                    "     ELSE 'Available' END AS [STATUS] " +
                    "FROM Products ORDER BY ProductName;";
                DataTable dt = new DataTable();
                new SqlDataAdapter(sql, conn).Fill(dt);
                return dt;
            }
        }
    }
}