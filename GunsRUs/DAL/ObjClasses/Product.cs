using System;
using System.Drawing;


namespace GunsRUs
{
    class Product
    {
        #region Fields
        private int productId;
        private int proIndex;
        private string proName;
        private string description;
        private decimal price;
        private bool isAvailable;
        private string color;
        private float proSize;
        private Image picture;
        private string productCategory;
        #endregion

        #region Properties
        
        public string Name { get => proName; set => proName = value; }
        public string Description { get => description; set => description = value; }
        public decimal Price { get => price; set => price = Math.Round(value,2); }
        public bool IsAvailable { get => isAvailable; set => isAvailable = value; }
        public string Color { get => color; set => color = value; }
        public float Size { get => proSize; set => proSize = value; }
        public Image Picture { get => picture; set => picture = value; }
        public string Category { get => productCategory; set => productCategory = value; }
        public int CategoryId { get => proIndex; set => proIndex = value; }
        public int ProductId { get => productId; set => productId = value; }

        #endregion

    }
}
