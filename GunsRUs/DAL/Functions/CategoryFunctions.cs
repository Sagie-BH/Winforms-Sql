using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GunsRUs
{
    class CategoryFunctions
    {
        private static SqlDBCon sqlDB = new SqlDBCon();

        /// <summary>
        /// Retrieves all the categories according to query constraint.
        /// Includes - "SELECT * FROM Category "
        /// </summary>
        /// <param name="query">Empty string for all categories</param>
        /// <returns></returns>
        public static List<Category> GetCategories(string query)
        {
            SqlConnection connection = sqlDB.Connection;

            List<Category> categories = new List<Category>();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Category " + query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Enum.TryParse((string)reader["Section"], out GunsRUsEnums.Section rSection);
                            byte[] bA = (byte[])reader["Picture"];
                            MemoryStream ms = new MemoryStream(bA);
                            Bitmap returnImage = new Bitmap(Image.FromStream(ms));
                            Category tempProduct = new Category
                            {
                                Id = (int)reader["CategoryId"],
                                Name = (string)reader["Name"],
                                Section = rSection,
                                CatImage = returnImage
                            };
                            categories.Add(tempProduct);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return categories;
        }


        /// <summary>
        /// Retrieves all categories
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategories()
        {
            SqlConnection connection = sqlDB.Connection;

            List<Category> categories = new List<Category>();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM Category ", connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Enum.TryParse((string)reader["Section"], out GunsRUsEnums.Section rSection);
                            byte[] bA = (byte[])reader["Picture"];
                            MemoryStream ms = new MemoryStream(bA);
                            Bitmap returnImage = new Bitmap(Image.FromStream(ms));
                            Category tempProduct = new Category
                            {
                                Id = (int)reader["CategoryId"],
                                Name = (string)reader["Name"],
                                Section = rSection,
                                CatImage = returnImage
                            };
                            categories.Add(tempProduct);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return categories;
        }


        /// <summary>
        /// Incharge of populating the Category list view
        /// Retrives all the categories
        /// Turns each Category Image to Image List Item with name
        /// Setting up the Image List in the List View
        /// </summary>
        /// <param name="imageList"></param>
        /// <param name="listView"></param>
        public static void PopulateListView(List<Category> dsplCategories, ListView listView)
        {
            listView.Clear();
            listView.BeginUpdate();

            ImageList imageList = new ImageList();

            foreach (Category category in dsplCategories)
            {
                string fixxedCatName = category.Name.Split('.').Last();
                ListViewItem item = new ListViewItem(Functions.StringConverter(fixxedCatName, '_', ' '));

                imageList.Images.Add(fixxedCatName, category.CatImage);

                int imgSize = listView.Height - listView.Height / 4;
                imageList.ImageSize = new Size(imgSize, imgSize);
                item.ImageKey = fixxedCatName;

                listView.Items.Add(item);
            }
            listView.LargeImageList = imageList;
            listView.EndUpdate();
        }


        /// <summary>
        /// Adding new Category to the database
        /// </summary>
        /// <param name="name">Category name</param>
        /// <param name="catSec">Section</param>
        /// <param name="imgPath">Image Path</param>
        public static void AddCategory(string name, string catSec, string imgPath)
        {
            using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
            {
                string addQuery = "INSERT INTO Category(Name, Section, Picture)" +
                                    "Values (@Name, @Section, @Picture) ";
                using (SqlCommand addCommand = new SqlCommand(addQuery, connection))
                {
                    addCommand.Parameters.AddWithValue("@Name", Functions.StringConverter(name, ' ', '_'));
                    addCommand.Parameters.AddWithValue("@Section", catSec.ToString());
                    addCommand.Parameters.AddWithValue("@Picture", File.ReadAllBytes(imgPath));     // Adding the image as byte[]
                    connection.Open();
                    addCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// Sets the categories in the list view by chosen section
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="listView"></param>
        public static void CategoriesBySection(ComboBox comboBox, ListView listView)
        {
            if (comboBox.SelectedValue != null)
            {
                if (comboBox.SelectedIndex == 0)
                {
                    PopulateListView(GetCategories(), listView);
                }
                if (comboBox.SelectedIndex == 1)
                {
                    PopulateListView(GetCategories("WHERE Section = 'HangGuns'"), listView);
                }
                if (comboBox.SelectedIndex == 2)
                {
                    PopulateListView(GetCategories("WHERE Section = 'Rifles'"), listView);
                }
                if (comboBox.SelectedIndex == 3)
                {
                    PopulateListView(GetCategories("WHERE Section = 'MachineGuns'"), listView);
                }
                if (comboBox.SelectedIndex == 4)
                {
                    PopulateListView(GetCategories("WHERE Section = 'ShotGuns'"), listView);
                }
                if (comboBox.SelectedIndex == 5)
                {
                    PopulateListView(GetCategories("WHERE Section = 'Equipment'"), listView);
                }
                if (comboBox.SelectedIndex == 6)
                {
                    PopulateListView(GetCategories("WHERE Section = 'Other'"), listView);
                }
            }
        }


        /// <summary>
        /// Deleting the selected categories from datebase and view list
        /// Repopulating the list view
        /// </summary>
        /// <param name="listView"></param>
        static public void CategoryListViewDelete(ListView listView)
        {
            foreach (ListViewItem item in listView.SelectedItems)
                if (listView.SelectedItems.Count > 0)
                    Functions.DeleteFromTable(item.ImageKey, "Category");
                else
                    MessageBox.Show("You must choose at least 1\ncategory to delete");
            PopulateListView(GetCategories(), listView);
        }


        /// <summary>
        /// Validate that there is only one selected item
        /// Updates the data base & the list view
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="categoryNewName"></param>
        /// <param name="catNewSection"></param>
        /// <param name="cateImgPath"></param>
        public static void UpdateCategory(ListView listView, string categoryNewName, GunsRUsEnums.Section catNewSection, string cateImgPath)
        {
            if (listView.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please Choose A Category To Update...");
            }

            if (listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Please Update One By One");
            }
            else
            {
                foreach (ListViewItem item in listView.SelectedItems)
                {

                    if (categoryNewName != string.Empty && cateImgPath != string.Empty)
                    {
                        using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
                        {
                            string updateCat = "Update Category Set Name = @Name," +
                                                    "Section = @Section, Picture = @Picture " +
                                                    "Where Name = '" + item.ImageKey + "'";
                            using (SqlCommand updateCommand = new SqlCommand(updateCat, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@Name", categoryNewName);
                                updateCommand.Parameters.AddWithValue("@Section", catNewSection.ToString());
                                updateCommand.Parameters.AddWithValue("@Picture", File.ReadAllBytes(cateImgPath));
                                connection.Open();
                                updateCommand.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }
                    else
                        MessageBox.Show("All Category fields must be filled!");
                }
            }
            PopulateListView(GetCategories(), listView);
        }


    }
}
