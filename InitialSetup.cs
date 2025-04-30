using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simon_Says
{
    public class InitialSetup
    {
        private static Dictionary<string, PictureBox> pictureBoxDictionary = new Dictionary<string, PictureBox>();

        // Returns a dictionary of all possible tiles
        public static Dictionary<string, string> DictOfAllTiles()
        {
            string combinePath = Path.Combine(InitializeRootPath.GetRootPath());

            string redTile = Path.Combine(combinePath, "png", "red_tile512.png");
            string blueTile = Path.Combine(combinePath, "png", "blue_tile512.png");
            string orangeTile = Path.Combine(combinePath, "png", "orange_tile512.png");
            string greenTile = Path.Combine(combinePath, "png", "green_tile512.png");
            string caribBlueTile = Path.Combine(combinePath, "png", "caribBlue_tile512.png");
            string greyTile = Path.Combine(combinePath, "png", "grey_tile512.png");
            string indigoTile = Path.Combine(combinePath, "png", "indigo_tile512.png");
            string maroonTile = Path.Combine(combinePath, "png", "maroon_tile512.png");
            string oliveTile = Path.Combine(combinePath, "png", "olive_tile512.png");
            string pinkTile = Path.Combine(combinePath, "png", "pink_tile512.png");

            Dictionary<string, string> dictOfAllTiles = new Dictionary<string, string>()
                                                    {
                                                        {"Red", redTile},
                                                        {"Blue", blueTile},
                                                        {"Orange", orangeTile},
                                                        {"Green", greenTile},
                                                        {"CaribBlue", caribBlueTile},
                                                        {"Grey", greyTile},
                                                        {"Indigo", indigoTile},
                                                        {"Maroon", maroonTile },
                                                        {"Olive", oliveTile},
                                                        {"Pink", pinkTile}
                                                    };
            return dictOfAllTiles;
        }

        // Initialize the PictureBox with a given tile and image path
        public static void InitializePictureBox(PictureBox pictureBox, string tile, string imagePath)
        {
            try
            {
                pictureBox.Image = Image.FromFile(imagePath);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.BackColor = Color.Transparent;
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Tag = tile;

                pictureBox.Click -= PlayersTurn; // Remove any previous attachment
                pictureBox.Click += PlayersTurn; // Attach event handler

                // Update the dictionary with the new PictureBox
                pictureBoxDictionary[tile] = pictureBox;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing PictureBox for tile {tile}: {ex.Message}");
                MessageBox.Show($"Error initializing PictureBox for tile {tile}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Define fixed positions for PictureBoxes
        public static Point SetFixedPositionPictureBoxes(int index)
        {
            // Define fixed positions based on the index
            switch (index)
            {
                case 0:
                    return new Point(13, 12);
                case 1:
                    return new Point(321, 12);
                case 2:
                    return new Point(13, 316);
                case 3:
                    return new Point(321, 316);
                default:
                    return Point.Empty; // Default position if index is out of range
            }
        }
    }

}
