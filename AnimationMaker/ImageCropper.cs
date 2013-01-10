using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AnimationMaker
{
    class ImageCropper
    {
        static public System.Windows.Controls.Image[] CropToImageControls(Bitmap SourceImage, int SpriteWidth, int SpriteHeight,int Amount)
        {
            int SourceImageWidth = SourceImage.Width;
            int SourceImageHeight = SourceImage.Height;

            int x = 0;
            int y = 0;

            System.Windows.Controls.Image[] Images = new System.Windows.Controls.Image[Amount];

            for (int i = 0; i < Amount; ++i)
            {
                if (x >= SourceImageWidth)
                {
                    y += SpriteHeight;
                    x = 0;
                }

                Bitmap Sprite = SourceImage.Clone(new RectangleF(x, y, SpriteWidth, SpriteHeight), SourceImage.PixelFormat);
                MemoryStream TempStream = new MemoryStream();

                Sprite.Save(TempStream, ImageFormat.Png);

                BitmapImage FinalSprite = new BitmapImage();
                FinalSprite.BeginInit();
                FinalSprite.StreamSource = TempStream;
                FinalSprite.EndInit();

                System.Windows.Controls.Image SpriteImage = new System.Windows.Controls.Image();
                SpriteImage.Source = FinalSprite;

                SpriteImage.Width = SpriteWidth;
                SpriteImage.Height = SpriteHeight;

                //Use the tag of the image to save the id.
                SpriteImage.Tag = SpriteSource.GID++;

                //Add sprite to array;
                Images[i] = SpriteImage;

                x += SpriteWidth;
            }

            return Images;
        }
    }
}
