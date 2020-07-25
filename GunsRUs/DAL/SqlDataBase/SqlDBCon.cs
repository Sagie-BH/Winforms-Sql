using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace GunsRUs
{
    class SqlDBCon
    {
        string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=DBGunRUs;Integrated Security=True;Connect Timeout=30;";

        public string ConnectionString { get => connectionString; set => connectionString = value; }

        public SqlConnection Connection = new SqlConnection();

        public SqlDBCon()
        {
            this.ConnectionString = connectionString;
            Connection = new SqlConnection(ConnectionString);
        }

        public SqlDBCon(string newSqlCon)
        {
            this.ConnectionString = newSqlCon;
            Connection = new SqlConnection(ConnectionString);
        }

        public void CheckSQlCon()
        {
            using (SqlConnection sql = new SqlConnection(ConnectionString))
            {
                try
                {
                    sql.Open();
                }
                catch
                {
                    DummyDB.BuildDB();
                    DummyDB.BuildTablesAndData();
                    //Application.Restart();
                    //Environment.Exit(0);
                }
            }
        }
    }
}
