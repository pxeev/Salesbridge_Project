using System;
using System.Data.SqlClient;

namespace Salesbridge
    public static class DatabaseHelper
    {
        private static readonly string MasterConnection =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";

        public static readonly string AppConnection =  //app connection (used for all normal queries)
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SalesbridgeDB;Integrated Security=True;";
        public static void InitializeDatabase()
        {
            using (SqlConnection conn = new SqlConnection(MasterConnection))  //create the database (connect to mstr)
        {
                conn.Open();
                string sql =
                    "IF NOT EXISTS " +
                    "(SELECT name FROM sys.databases WHERE name = N'SalesbridgeDB') " +
                    "CREATE DATABASE SalesbridgeDB;";
                new SqlCommand(sql, conn).ExecuteNonQuery();
            }

            using (SqlConnection conn = new SqlConnection(AppConnection)) //create the users table inside SalesbridgeDB
        {
                conn.Open();
                string sql =
                    "IF NOT EXISTS " +
                    "(SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U') " +
                    "CREATE TABLE Users ( " +
                    "    Id       INT IDENTITY(1,1) PRIMARY KEY, " +
                    "    Username NVARCHAR(100)  NOT NULL, " +
                    "    Email    NVARCHAR(200)  NOT NULL UNIQUE, " +
                    "    Password NVARCHAR(200)  NOT NULL " +
                    ");";
                new SqlCommand(sql, conn).ExecuteNonQuery();
            }
        }

        public static bool ValidateUser(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql =
                    "SELECT COUNT(*) FROM Users " +
                    "WHERE Email = @Email AND Password = @Password;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email",    email.Trim());
                cmd.Parameters.AddWithValue("@Password", password);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
        public static bool EmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
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
                    string sql =
                        "INSERT INTO Users (Username, Email, Password) " +
                        "VALUES (@Username, @Email, @Password);";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Username", username.Trim());
                    cmd.Parameters.AddWithValue("@Email",    email.Trim());
                    cmd.Parameters.AddWithValue("@Password", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public static string GetUsernameByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(AppConnection))
            {
                conn.Open();
                string sql = "SELECT Username FROM Users WHERE Email = @Email;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Email", email.Trim());
                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }
    }
}
