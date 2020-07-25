using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GunsRUs
{
    class ProductFunctions
    {
        static SqlDBCon sqlDB = new SqlDBCon();


        /// <summary>
        /// Retrieves all products
        /// </summary>
        /// <returns></returns>
        public static List<Product> GetProducts()
        {
            SqlConnection connection = sqlDB.Connection;

            List<Product> products = new List<Product>();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                string getAll = "SELECT * FROM Product";
                using (SqlCommand command = new SqlCommand(getAll, connection))
                {
                    
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            byte[] bA = (byte[])reader["Picture"];
                            MemoryStream ms = new MemoryStream(bA);
                            Bitmap returnImage = new Bitmap(Image.FromStream(ms), 100, 100);
                            Product tempProduct = new Product
                            {
                                ProductId = (int)reader["ProductId"],
                                Name = Functions.StringConverter((string)reader["Name"], '_', ' '),
                                Picture = returnImage,
                                CategoryId = (int)reader["Category"],
                                Description = Functions.StringConverter((string)reader["Description"], '_', ' '),
                                Color = Functions.StringConverter((string)reader["Color"], '_', ' '),
                                IsAvailable = (bool)reader["Available"],
                                Size = Convert.ToSingle(reader["Size"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Category = GetCatName((int)reader["Category"])
                            };
                            products.Add(tempProduct);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return products;
        }


        /// <summary>
        /// Retrieves all the products according to query constraint
        /// </summary>
        /// <param name="query">Empty string for all categories</param>
        /// <returns></returns>
        public static List<Product> GetProducts(string query)
        {
            SqlConnection connection = sqlDB.Connection;

            List<Product> products = new List<Product>();
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] bA = (byte[])reader["Picture"];
                            MemoryStream ms = new MemoryStream(bA);
                            Bitmap returnImage = new Bitmap(Image.FromStream(ms), 100, 100);
                            Product tempProduct = new Product
                            {
                                ProductId = (int)reader["ProductId"],
                                Name = (string)reader["Name"],
                                Picture = returnImage,
                                Description = (string)reader["Description"],
                                Color = (string)reader["Color"],
                                IsAvailable = (bool)reader["Available"],
                                Size = Convert.ToSingle(reader["Size"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Category = GetCatName((int)reader["Category"])
                            };
                            products.Add(tempProduct);
                        }
                        reader.Close();
                    }
                    connection.Close();
                }
            }
            return products;
        }

        static private string GetCatName(int _id)
        {
            return CategoryFunctions.GetCategories().Where(x => x.Id == _id).Select(x => x.Name).FirstOrDefault();
        }


        /// <summary>
        /// Adds an item to Product table
        /// Validates all properties are filled
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="isAvailable"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="catName"></param>
        /// <param name="imgPath"></param>
        public static void AddProduct(string name, string description, decimal price, bool isAvailable,
                                      string color, float size, string catName, string imgPath)
        {

            using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
            {
                string addQuery = "INSERT INTO Product (Name, Description, Price, Available," +
                                  " Color, Size, Category, Picture)" +
                                  " Values (@Name, @Description, @Price, @Available, @Color," +
                                  " @Size, @Category, @Picture)";
                
                using (SqlCommand addCommand = new SqlCommand(addQuery, connection))
                {
                    addCommand.Parameters.AddWithValue("@Name", Functions.StringConverter(name, ' ', '_'));
                    addCommand.Parameters.AddWithValue("@Description", Functions.StringConverter(description, ' ', '_'));
                    addCommand.Parameters.AddWithValue("@Price", price);
                    addCommand.Parameters.AddWithValue("@Available", isAvailable);
                    addCommand.Parameters.AddWithValue("@Color", Functions.StringConverter(color, ' ', '_'));
                    addCommand.Parameters.AddWithValue("@Size", size);
                    addCommand.Parameters.AddWithValue("@Category", catName);
                    addCommand.Parameters.AddWithValue("@Picture", File.ReadAllBytes(imgPath));     // Adding the image as byte[]
                    connection.Open();
                    addCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }



        /// <summary>
        /// Creating a product list according to selected categories
        /// </summary>
        /// <param name="categoryListView"></param>
        /// <returns></returns>
        public static List<Product> ProductDisplayByCategory(ListView categoryListView)
        {
            List<Product> resultProList = new List<Product>();

            if (categoryListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in categoryListView.SelectedItems)
                {
                    foreach (Category cat in CategoryFunctions.GetCategories())
                    {
                        if (cat.Name == Functions.StringConverter(item.Text, ' ', '_'))     // Fix the syntax
                        {
                            var buildIt = GetProducts("Select * from Product " +       // Multi Category selection
                                                      "Where Product.Category = '" + cat.Id + "'");
                            foreach (Product temp in buildIt)
                            {
                                resultProList.Add(temp);
                            }
                        }
                    }
                }
            }
            else
                resultProList = GetProducts();

            return resultProList;
        }



        /// <summary>
        /// Updates the selected row in the data base
        /// </summary>
        /// <param name="ProDvg"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="price"></param>
        /// <param name="isAvailable"></param>
        /// <param name="color"></param>
        /// <param name="size"></param>
        /// <param name="category"></param>
        /// <param name="imgPath"></param>
        /// <param name="proToUpdateId"></param>
        public static void UpdateProduct(DataGridView ProDvg, string name, string description, decimal price, bool isAvailable,
                                      string color, float size, int category, string imgPath, int proToUpdateId)
        {
            if (ProDvg.CurrentRow == null)
                MessageBox.Show("Please Choose A Product To Update...");

            else if (ProDvg.SelectedRows.Count > 1)
                MessageBox.Show("Please Update One By One");
            else
            {
                if (name != string.Empty || description != string.Empty || price != 0 ||
                              color != string.Empty || size != 0 || imgPath != string.Empty)
                {
                    using (SqlConnection connection = new SqlConnection(sqlDB.ConnectionString))
                    {
                        string proUpdate = "Update Product Set Name = @Name, Description = @Description," +
                            " Price = @Price, Available = @Available, Color = @Color, Size = @Size, Picture = @Picture," +
                            " Category = @Category Where Product.ProductId = " + proToUpdateId;

                        using (SqlCommand updateCommand = new SqlCommand(proUpdate, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@Name", name);
                            updateCommand.Parameters.AddWithValue("@Description", description);
                            updateCommand.Parameters.AddWithValue("@Price", price);
                            updateCommand.Parameters.AddWithValue("@Available", isAvailable);
                            updateCommand.Parameters.AddWithValue("@Color", color);
                            updateCommand.Parameters.AddWithValue("@Size", size);
                            updateCommand.Parameters.AddWithValue("@Category", category);
                            updateCommand.Parameters.AddWithValue("@Picture", File.ReadAllBytes(imgPath));
                            connection.Open();
                            updateCommand.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                }
                else
                    MessageBox.Show("All Category fields must be filled!");
                ProDvg.DataSource = GetProducts();
            }
        }



        /// <summary>
        /// Fill the product data grid view according to the price range
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="productDVG"></param>
        public static void ProductByPrice(string low, string high, DataGridView productDVG)
        {
            productDVG.DataSource = GetProducts();
            if (low != "" || high != "")
            {
                if (string.IsNullOrEmpty(high) || string.IsNullOrEmpty(low))
                {
                    if (string.IsNullOrEmpty(low))
                    {
                        productDVG.DataSource = GetProducts
                            ("Select * From Product Where Product.Price < " + high);
                    }
                    if (string.IsNullOrEmpty(high))
                    {
                        productDVG.DataSource = GetProducts
                            ("Select * From Product Where Product.Price > " + low);
                    }
                }
                else
                {
                    productDVG.DataSource = GetProducts
                        ("Select * From Product Where Product.Price > "
                        + low + " And Product.Price < " + high);
                }
            }
        }
    }
}
