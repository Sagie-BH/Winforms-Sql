using System;
using System.Windows.Forms;

namespace GunsRUs
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SqlDBCon sqlDBCon = new SqlDBCon();
            sqlDBCon.CheckSQlCon();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GunRUsForm());
        }
    }
}
