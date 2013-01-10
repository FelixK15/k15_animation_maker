using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.IO;

namespace AnimationMaker
{
    /// <summary>
    /// Interaction logic for SpritesheetImporter.xaml
    /// </summary>
    public partial class SpritesheetImporter : Window
    {
        private MainWindow MainWindow { get; set; }

        public SpritesheetImporter(MainWindow window)
        {
            MainWindow = window;
            InitializeComponent();
        }

        private void txtNumberChecker(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]))
            {
                e.Handled = true;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog SearchDialog = new OpenFileDialog();
            SearchDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
            if (SearchDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String FileName = SearchDialog.FileName;
                try
                {
                    BitmapImage FileImage = new BitmapImage(new Uri(FileName, UriKind.Absolute));

                    lbHeight.Content = FileImage.PixelHeight + "px";
                    lbWidth.Content = FileImage.PixelWidth + "px";

                    txFile.Text = SearchDialog.FileName;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (txSprites.Text == "" || txWidth.Text == "" || txHeight.Text == "")
            {
                System.Windows.MessageBox.Show(this, "Missing properties.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    Bitmap SpriteSheet = new Bitmap(txFile.Text);
                    
                    int AmountSprites = Convert.ToInt32(txSprites.Text);
                    int SpriteWidth = Convert.ToInt32(txWidth.Text);
                    int SpriteHeight = Convert.ToInt32(txHeight.Text);

                    MainWindow.CurrentAnimation.SpriteSources.Add(new SpriteSheetSource(System.IO.Path.GetFileName(txFile.Text),
                                                                    SpriteWidth,SpriteHeight,AmountSprites));

                    System.Windows.Controls.Image[] Images = ImageCropper.CropToImageControls(SpriteSheet, SpriteWidth, SpriteHeight, AmountSprites);

                    for (int i = 0; i < AmountSprites; ++i)
                    {
                        MainWindow.AddSprite(Images[i]);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
