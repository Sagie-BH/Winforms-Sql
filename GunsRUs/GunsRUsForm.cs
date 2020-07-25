using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;



namespace GunsRUs
{
    public partial class GunRUsForm : MetroFramework.Forms.MetroForm
    {
        public GunRUsForm()
        {
            InitializeComponent();
            InitializeData();
        }



        /// <summary>
        /// Initialize data to the form
        /// </summary>
        private void InitializeData()
        {
            // Category section comboBox data source    -    GunsRUsEnums
            catSectionCB.DataSource = Enum.GetValues(typeof(GunsRUsEnums.Section)).Cast<GunsRUsEnums.Section>().Where(x => x != GunsRUsEnums.Section.AllSections).ToList();

            // Manager list view display
            CategoryFunctions.PopulateListView(CategoryFunctions.GetCategories(), ManagerlistView);

            // Shop list view display
            CategoryFunctions.PopulateListView(CategoryFunctions.GetCategories(), ShopListView);

            // Manager Main section comboBox data source    -    GunsRUsEnums
            ManagerMainSectionCB.DataSource = Enum.GetValues(typeof(GunsRUsEnums.Section));

            // Category Id combo box data source
            pMCatNameCB.DataSource = CategoryFunctions.GetCategories().Select(x => x.Name).ToArray();

            // Slash Picture Box
            slashPicBox.Image = Properties.Resources.Slash__1_;
            slashPicBox.SizeMode = PictureBoxSizeMode.StretchImage;

            // Product quantity label
            QuantityTxtBox.Text = ProductFunctions.GetProducts().Count.ToString();

            // Shop Main section comboBox
            ShopMainSecCb.DataSource = Enum.GetValues(typeof(GunsRUsEnums.Section));

            // SHop Data Grid
            ShopProGrid.DataSource = ProductFunctions.GetProducts();
            ShopProGrid.RowTemplate.Height = 100;

            // SHop Data Grid
            ManagerDataGrid.DataSource = ProductFunctions.GetProducts();
            ManagerDataGrid.RowTemplate.Height = 30;
        }


        #region Buttons

        /// <summary>
        /// Opens file dialog to locate image for new category
        /// Hides & Reveals preview text box accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategoryPicPath_Click(object sender, EventArgs e)
        {
            catPicBox.ImageLocation = Functions.ImageFromFile(catPicBox);

        }


        /// <summary>
        /// Adds a new category to the database using user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategoryAdd_Click(object sender, EventArgs e)
        {
            if (catPicBox.Image != null && !string.IsNullOrEmpty(catName.Text))
            {
                if ((GunsRUsEnums.Section)catSectionCB.SelectedItem != GunsRUsEnums.Section.AllSections)
                {
                    CategoryFunctions.AddCategory(catName.Text,
                                        catSectionCB.Text,
                                        catPicBox.ImageLocation);
                    CategoryFunctions.PopulateListView(CategoryFunctions.GetCategories("SELECT * FROM Category"), ManagerlistView);

                    // Category Id combo box data source
                    pMCatNameCB.DataSource = CategoryFunctions.GetCategories().Select(x => x.Id).ToArray();
                }
                else
                    messageLabel.Text = "Can't add to All Sections, Choose a section";
            }
            else messageLabel.Text = "Must choose a name and load image first";
        }


        /// <summary>
        /// Deletes all selected categories
        /// Reseting the list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategoryDelete_Click(object sender, EventArgs e)
        {
            CategoryFunctions.CategoryListViewDelete(ManagerlistView);
        }


        /// <summary>
        /// Updates selected category in the data base and list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategoryUpdate_Click(object sender, EventArgs e)
        {
            if ((GunsRUsEnums.Section)catSectionCB.SelectedItem != GunsRUsEnums.Section.AllSections)
            {
                CategoryFunctions.UpdateCategory(ManagerlistView, catName.Text, (GunsRUsEnums.Section)catSectionCB.SelectedItem, catPicBox.ImageLocation);
            }
            else
                messageLabel.Text = "Can't update a category to All Sections,\nChoose a section";
        }


        /// <summary>
        /// Opens file dialog to upload photo
        /// Attach selected image to product pidcture view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProductPath_Click(object sender, EventArgs e)
        {
            proPicBox.ImageLocation = Functions.ImageFromFile(proPicBox);
        }


        /// <summary>
        /// Validates user input types
        /// Adds a now row to product table
        /// Repopulate category list view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProAdd_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(proNameTxtBox.Text) && !string.IsNullOrEmpty(proDescTextBox.Text) &&
                !string.IsNullOrEmpty(proColorTxtBox.Text) && !string.IsNullOrEmpty(pMCatIdText.Text) && !string.IsNullOrEmpty(proPicBox.ImageLocation))
            {
                if (!decimal.TryParse(proPriceTxtBox.Text, out decimal price))
                    messageProLabel.Text = "Price Must Be A Number";
                else if (!float.TryParse(proSizeTxtBox.Text, out float size))
                    messageProLabel.Text = "Size Must Be A Number";
                else
                {
                    ProductFunctions.AddProduct(proNameTxtBox.Text, proDescTextBox.Text, price, proAvailableCheckB.Checked, proColorTxtBox.Text,
                                             size, pMCatIdText.Text, proPicBox.ImageLocation);
                    messageProLabel.Text = "Added successfully";
                    CleanProText();
                }
            }
            else
                messageProLabel.Text = "Missing Details...";

            ManagerDataGrid.DataSource = ProductFunctions.GetProducts();

        }


        /// <summary>
        /// Product update Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProUpdate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(proNameTxtBox.Text) && !string.IsNullOrEmpty(proDescTextBox.Text) &&
                !string.IsNullOrEmpty(proColorTxtBox.Text) && !string.IsNullOrEmpty(pMCatIdText.Text) && !string.IsNullOrEmpty(proPicBox.ImageLocation))
            {
                if (!decimal.TryParse(proPriceTxtBox.Text, out decimal price))
                    messageProLabel.Text = "Price Must Be A Number";
                else if (!float.TryParse(proSizeTxtBox.Text, out float size))
                    messageProLabel.Text = "Size Must Be A Number";
                else
                {
                    Product toUpdate = (Product)ManagerDataGrid.CurrentRow.DataBoundItem;

                    ProductFunctions.UpdateProduct(ManagerDataGrid, proNameTxtBox.Text, proDescTextBox.Text, price,
                    proAvailableCheckB.Checked, proColorTxtBox.Text, size, int.Parse(pMCatIdText.Text), proPicBox.ImageLocation, toUpdate.ProductId);

                    messageProLabel.Text = "Updated successfully";
                    CleanProText();
                }
            }
            else
                messageProLabel.Text = "Missing Details...";


        }


        /// <summary>
        /// Deletes a row from the database and the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProDelete_Click(object sender, EventArgs e)
        {

            Product toDelete = (Product)ManagerDataGrid.CurrentRow.DataBoundItem;

            if (toDelete != null)
            {
                Functions.DeleteFromTable(toDelete.ProductId, "Product");
            }
            else
            {
                messageProLabel.Text = "Choose Products To Delete";
            }

            ManagerDataGrid.DataSource = ProductFunctions.GetProducts();
        }

        #endregion



        #region ComboBoxes Display Control

        /// <summary>
        /// Manager section combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainSectionCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ManagerMainSectionCB.SelectedItem != null || ManagerMainSectionCB.SelectedIndex != 0)
            {
                CategoryFunctions.CategoriesBySection(ManagerMainSectionCB, ManagerlistView);
                CategoryFunctions.CategoriesBySection(ManagerMainSectionCB, ManagerlistView);
                ManagerDataGrid.DataSource = ProductFunctions.GetProducts().Where(x => x.Category == ManagerMainSectionCB.SelectedValue.ToString()).ToList();
            }
            else
                ManagerDataGrid.DataSource = ProductFunctions.GetProducts();
        }


        /// <summary>
        /// Shop category listview selected by categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShopSectionCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShopMainSecCb.SelectedItem != null || ShopMainSecCb.SelectedIndex != 0)
            {
                CategoryFunctions.CategoriesBySection(ShopMainSecCb, ShopListView);
                CategoryFunctions.CategoriesBySection(ShopMainSecCb, ShopListView);
                ShopProGrid.DataSource = ProductFunctions.GetProducts().Where(x => x.Category == ShopMainSecCb.SelectedValue.ToString()).ToList();
            }
            else
                ShopProGrid.DataSource = ProductFunctions.GetProducts();
        }



        /// <summary>
        /// Incharge of filling the product data grid according to selected categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManagerDataGrid.DataSource = ProductFunctions.ProductDisplayByCategory(ManagerlistView);
        }

        #endregion



        #region Products Data Grids

        /// <summary>
        /// Shop Products Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mProCatListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShopProGrid.DataSource = ProductFunctions.ProductDisplayByCategory(ShopListView);

            if (ManagerlistView.SelectedItems.Count > 0)
                QuantityTxtBox.Text = ProductFunctions.ProductDisplayByCategory(ManagerlistView).Count.ToString();
            else
                QuantityTxtBox.Text = ProductFunctions.GetProducts().Count.ToString();

            // Product quantity label
            QuantityTxtBox.Text = ShopProGrid.Rows.Count.ToString();
        }

        /// <summary>
        /// Manager Product Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCatListVew_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManagerDataGrid.DataSource = ProductFunctions.ProductDisplayByCategory(ManagerlistView);
        }


        #endregion



        #region Helpers

        /// <summary>
        /// Changes category read only text box acording to selected category id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProCatIndexCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pMCatNameCB.SelectedItem != null)
                pMCatIdText.Text = CategoryFunctions.GetCategories().
                                     Where(x => x.Name == pMCatNameCB.Text).
                                     Select(x => x.Id.ToString()).FirstOrDefault();

        }


        /// <summary>
        /// Displays products between selected range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Price_TextChanged(object sender, EventArgs e)
        {
            ProductFunctions.ProductByPrice(txtLowPrice.Text, txtMaxPrice.Text, ShopProGrid);
        }


        /// <summary>
        /// Cleans the product text fields
        /// </summary>
        private void CleanProText()
        {
            proNameTxtBox.Text = string.Empty;
            proColorTxtBox.Text = string.Empty;
            proDescTextBox.Text = string.Empty;
            proPriceTxtBox.Text = string.Empty;
            proSizeTxtBox.Text = string.Empty;
        }

        #endregion


    }
}