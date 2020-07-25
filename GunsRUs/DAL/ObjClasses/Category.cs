using System.Drawing;


namespace GunsRUs
{
    class Category
    {
        #region
        private int id;
        private string name;
        private Image catImage;
        private GunsRUsEnums.Section section;
        #endregion

        #region
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public Image CatImage { get => catImage; set => catImage = value; }
        internal GunsRUsEnums.Section Section { get => section; set => section = value; }
        #endregion
    }
}
