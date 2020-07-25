using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace GunsRUs
{
    class Functions
    {
        static SqlDBCon sqlDB = new SqlDBCon();


        /// <summary>
        /// Text fixer - convert string to char[]
        /// </summary>
        /// <param name="strToCon"></param>
        /// <returns></returns>
        public static string StringConverter(string strToCon, char cToCon, char result)
        {
            char[] vs = strToCon.ToCharArray();
            for (int i = 0; i < vs.Length; i++)
            {
                if (vs[i] == cToCon)
                    vs[i] = result;    // Change the char
            }
            return new string(vs);
        }


        /// <summary>
        /// Setting up image in the picture box and return the full path of the file
        /// </summary>
        /// <param name="path">Full Path</param>
        /// <param name="previewBox">Target Picture Box</param>
        public static string ImageFromFile(PictureBox previewBox)
        {
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                previewBox.Image = new Bitmap(open.FileName);
            }
            string path = open.FileName;
            return path;

        }



        /// <summary>
        /// Deletes one row from a database table -  by object name(string)
        /// </summary>
        /// <param name="rowName">Selected category to delete</param>
        /// <param name="tableName">Objective teble</param>
        public static void DeleteFromTable(string rowName, string tableName)
        {
            SqlDBCon sqlDB = new SqlDBCon();
            using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("DELETE from " + tableName + " where Name = '" + rowName + "'", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }



        /// <summary>
        /// Deletes one row from a database table -  by object name(int)
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="tableName"></param>
        public static void DeleteFromTable(int rowId, string tableName)
        {
            SqlDBCon sqlDB = new SqlDBCon();
            using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("DELETE from " + tableName +
                                                          " Where " + tableName + "." + tableName +"Id = '" + rowId + "'", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
