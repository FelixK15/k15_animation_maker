using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.IO;

namespace AnimationMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _ShowOnionLayer;

        public bool ShowOnionLayer
        {
            get { return _ShowOnionLayer; }
            set
            {
                if (value == true)
                {
                    CreateOnionLayer();
                }
                else
                {
                    ClearOnionLayer();
                }
                
                _ShowOnionLayer = value;
            }
        }

        public AnimationWindow AnimationWindow { get; set; }
        public Animation CurrentAnimation { get; set; }
        public int CurrentFrameIndex { get; set; }
        public Image SelectedSprite { get; set; }

        public MainWindow()
        {
            SelectedSprite = null;
            CurrentFrameIndex = 0;

            //Create a new animation on start and add an empty frame.
            CurrentAnimation = new Animation();
            CurrentAnimation.Frames.Add(new Frame());

            AnimationWindow = new AnimationWindow();
            AnimationWindow.Show();

            InitializeComponent();
            
            //Draw the center lines when all components have been initialized
            DrawCenterLines();
        }

        public void CreateOnionLayer()
        {
            //Check if there's a previous frame to show
            if (CurrentFrameIndex == 0)
            {
                return;
            }

            //Get the previous frame and make all its images transparent.
            Frame PreviousFrame = CurrentAnimation.Frames.ElementAt<Frame>(CurrentFrameIndex - 1);
            foreach (Sprite s in PreviousFrame.Sprites)
            {
                s.Image.Opacity = 0.5;
                cvAnimationBoard.Children.Add(s.Image);
                Canvas.SetLeft(s.Image, s.X + ((cvAnimationBoard.Width*0.5) - (s.Image.Width*0.5)));
                Canvas.SetTop(s.Image, s.Y + ((cvAnimationBoard.Height * 0.5) - (s.Image.Height * 0.5)));
            }
        }

        public void ClearOnionLayer()
        {
            List<UIElement> ElementsToRemove = new List<UIElement>();

            foreach (UIElement u in cvAnimationBoard.Children)
            {
                if (u.Opacity < 1.0)
                {
                    u.Opacity = 1.0;
                    ElementsToRemove.Add(u);
                }
            }

            foreach (UIElement u in ElementsToRemove)
            {
                cvAnimationBoard.Children.Remove(u);
            }
        }

        public void AddNewFrame()
        {
            Frame NewFrame = new Frame();
            CurrentAnimation.Frames.Add(NewFrame);
            ChangeFrame(NewFrame);
            String FrameCount = CurrentAnimation.Frames.Count.ToString();
            UpdateControls();
        }

        public void DeleteCurrentFrame()
        {
            if(MessageBox.Show("Do you really want to delete the current frame?","Question",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //Check if the user wants to delete the last frame of the animation...
                if (CurrentAnimation.Frames.Count <= 1)
                {
                    //...if he wants, stop him.
                    MessageBox.Show("This is the last frame of the animation.\nYou can't delete the last frame of an animation", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    //...if not, continue
                    
                    //Delete the current frame from the list
                    CurrentAnimation.Frames.RemoveAt(CurrentFrameIndex);

                    //Get the last frame of the list and set it as the current frame.
                    Frame NewFrame = CurrentAnimation.Frames.Last<Frame>();
                    ChangeFrame(NewFrame);
                }
            }
        }

        public void ChangeFrame(Frame Frame)
        {
            //Get the position of the provided frame within the framelist of the animation
            int NewFrameIndex = CurrentAnimation.Frames.FindIndex(f => f == Frame);

            //Check whether or not the provided frame is part of
            //the current animation.
            if (NewFrameIndex >= 0)
            {
                //A new list that just holds the elements, we want to remove from the
                //Canvas, as you can't remove elements within a foreach loop
                List<UIElement> ElementsToRemove = new List<UIElement>();

                //Add all images to the ElementsToRemove list.
                foreach (UIElement u in cvAnimationBoard.Children)
                {
                    if (u is Image)
                    {
                        ElementsToRemove.Add(u);
                    }
                }

                //Remove all images from the canvas.
                foreach (UIElement u in ElementsToRemove)
                {
                    cvAnimationBoard.Children.Remove(u);
                }

                //Add all images from the frame to the canvas.
                foreach (Sprite s in Frame.Sprites)
                {
                    cvAnimationBoard.Children.Add(s.Image);
                    Canvas.SetLeft(s.Image, (s.X + ((cvAnimationBoard.Width*0.5) - (s.Image.Width*0.5))));
                    Canvas.SetTop(s.Image, (s.Y + ((cvAnimationBoard.Height*0.5) - (s.Image.Height*0.5))));
                }

                //Clear the onion layer.
                ClearOnionLayer();

                //Set the new frame as the current frame.
                CurrentFrameIndex = NewFrameIndex;

                if (_ShowOnionLayer)
                {
                    CreateOnionLayer();
                }

                //without this if, the program crashes during InitializeComponent
                UpdateControls();
            }
        }

        public void AddSprite(Image Sprite)
        {
            //Adding a new sprite include the following operations:
            //1. Create a new Border control for the sprite selector.
            //2. Set all the handler to the Border control, which includes:
            //      1. Drawing a red border if the mouse is within a Border control
            //      2. Create a copy of the image and add it to the canvas, when the user 
            //         clicked on an image.
            //      3. Add a contextmenu to each image copy for deleting / moving to center
            //      4. Create a new Sprite object, set the image and add it to the current frame.

            // first step, create the Border control
            Border SpriteBorder = new Border();

            double SpriteWidth = Sprite.Width;
            double SpriteHeight = Sprite.Height;

            //Set its size so it fits into the stackpanel
            SpriteBorder.Width = (Sprite.Width * spSprites.Height / Sprite.Height);
            SpriteBorder.Height = spSprites.Height;

            if (Sprite.Width > SpriteBorder.Width)
            {
                Sprite.Width = SpriteBorder.Width;
            }

            if (Sprite.Height > SpriteBorder.Height)
            {
                Sprite.Height = SpriteBorder.Height;
            }

            SpriteBorder.Child = Sprite;
            SpriteBorder.BorderThickness = new Thickness(3);

            //Make the border invisible.
            SpriteBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

            //Add the handler for what happens when the mouse hovers over the border (Border gets red.)
            SpriteBorder.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                SpriteBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            };

            //Add the handler for what happens when the mouse leaves the border (Border gets invisible.)
            SpriteBorder.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                SpriteBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };

            //Add handler for what happens when the user clicks on the border control.
            SpriteBorder.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                Image CopyImage = new Image();
                Sprite NewSprite = new Sprite();

                NewSprite.ID = (int)Sprite.Tag;

                //Create a copy of the input sprite.
                CopyImage.BeginInit();
                CopyImage.Source = Sprite.Source;
                CopyImage.EndInit();

                //Set width and height.
                CopyImage.Width = SpriteWidth;
                CopyImage.Height = SpriteHeight;

                //Add move behavior to the image, so the user can drag the image over the canvas
                CopyImage.MouseLeftButtonDown += delegate(object o, MouseButtonEventArgs ev)
                {
                    SelectedSprite = CopyImage;
                };

                CopyImage.MouseLeftButtonUp += delegate(object o, MouseButtonEventArgs ev)
                {
                    SelectedSprite = null;
                    //Save position of the sprite after is has been moved.

                    if (CurrentFrameIndex < CurrentAnimation.Frames.Count)
                    {
                        Frame CurrentAnimationFrame = CurrentAnimation.Frames[CurrentFrameIndex];

                        //Remove it from the list.
                        CurrentAnimationFrame.Sprites.Remove(NewSprite);

                        //Save the new position.
                        NewSprite.X = Canvas.GetLeft(NewSprite.Image) - ((cvAnimationBoard.Width*0.5) - (NewSprite.Image.Width*0.5));
                        NewSprite.Y = Canvas.GetTop(NewSprite.Image) - ((cvAnimationBoard.Height*0.5) - (NewSprite.Image.Height*0.5));

                        //Re-add the sprite to the list.
                        CurrentAnimationFrame.Sprites.Add(NewSprite);
                    }                 
                };

                cvAnimationBoard.Children.Add(CopyImage);
                
                //Calculate center position of this sprite and set it.
                double Left = (cvAnimationBoard.Width / 2) - (CopyImage.Width / 2);
                double Top = (cvAnimationBoard.Height / 2) - (CopyImage.Height / 2);
                Canvas.SetLeft(CopyImage, Left);
                Canvas.SetTop(CopyImage, Top);

                //The Sprites X and Y Position are top left
                NewSprite.X = 0;
                NewSprite.Y = 0;

                //Create a new context menu for this sprite.
                ContextMenu SpriteMenu = new ContextMenu();

                //Add "move to center" item.
                MenuItem MoveToCenter = new MenuItem();
                MoveToCenter.Header = "Move to center";
                MoveToCenter.Click += delegate(object o, RoutedEventArgs ev)
                {
                    Canvas.SetLeft(CopyImage, Left);
                    Canvas.SetTop(CopyImage, Top);
                };

                //Add "Delete sprite" item.
                MenuItem DeleteSprite = new MenuItem();
                DeleteSprite.Header = "Delete sprite";
                DeleteSprite.Click += delegate(object o, RoutedEventArgs ev)
                {
                    //Delete the image from the canvas and the sprite object from the current frame.
                    cvAnimationBoard.Children.Remove(CopyImage);
                    CurrentAnimation.Frames[CurrentFrameIndex].Sprites.Remove(NewSprite);
                };

                //Add items to contextmenu
                SpriteMenu.Items.Add(MoveToCenter);
                SpriteMenu.Items.Add(DeleteSprite);

                //Add contextmenu to sprite.
                CopyImage.ContextMenu = SpriteMenu;

                //Add sprite to frame.
                NewSprite.Image = CopyImage;
                CurrentAnimation.Frames[CurrentFrameIndex].Sprites.Add(NewSprite);
            };

            //Add the border control to the stack panel.
            spSprites.Children.Add(SpriteBorder);
            spSprites.Width += SpriteBorder.Width;
        }

        private void DrawCenterLines()
        {
            Line HorizontalCenterLine = new Line();
            Line VerticalCenterLine = new Line();

            HorizontalCenterLine.Stroke = Brushes.Green;
            VerticalCenterLine.Stroke = Brushes.Green;

            HorizontalCenterLine.StrokeThickness = 1;
            VerticalCenterLine.StrokeThickness = 1;

            HorizontalCenterLine.SnapsToDevicePixels = true;
            VerticalCenterLine.SnapsToDevicePixels = true;

            HorizontalCenterLine.X1 = 0;
            HorizontalCenterLine.X2 = cvAnimationBoard.Width;
            HorizontalCenterLine.Y1 = cvAnimationBoard.Height / 2;
            HorizontalCenterLine.Y2 = cvAnimationBoard.Height / 2;

            VerticalCenterLine.X1 = cvAnimationBoard.Width / 2;
            VerticalCenterLine.X2 = cvAnimationBoard.Width / 2;
            VerticalCenterLine.Y1 = 0;
            VerticalCenterLine.Y2 = cvAnimationBoard.Height;

            cvAnimationBoard.Children.Add(HorizontalCenterLine);
            cvAnimationBoard.Children.Add(VerticalCenterLine);
        }

        private void cvAnimationBoard_MouseMove(object sender, MouseEventArgs e)
        {
            //refresh the x and y position relative to the center of the canvas
            Point MousePos = e.MouseDevice.GetPosition(cvAnimationBoard);
            Point Center = new Point(cvAnimationBoard.Width / 2, cvAnimationBoard.Height / 2);
            lbPosition.Content = "x:" + (MousePos.X - Center.X) + "; y:" + (MousePos.Y - Center.Y);

            //If a sprite has been selected, move the center of the sprite
            //to the mouse position
            if (SelectedSprite != null)
            {
                double Left = MousePos.X - (SelectedSprite.Width / 2);
                double Top = MousePos.Y - (SelectedSprite.Height / 2);
                Canvas.SetLeft(SelectedSprite, Left);
                Canvas.SetTop(SelectedSprite, Top);
            }
        }

        private void ClearSprites()
        {
            spSprites.Children.Clear();
        }

        private void ShowAnimationWindow()
        {
            if (!AnimationWindow.IsLoaded)
            {
                AnimationWindow = new AnimationWindow();
                AnimationWindow.Show();
            }
        }

        private void UpdateControls()
        {
            if (lbMaxFrame != null)
            {
                lbMaxFrame.Content = CurrentAnimation.Frames.Count;
            }

            if (slFrames != null)
            {
                slFrames.Maximum = CurrentAnimation.Frames.Count;
                slFrames.Value = (CurrentFrameIndex + 1);
            }

            if (lbCurrentFrame != null)
            {
                lbCurrentFrame.Content = (CurrentFrameIndex + 1);
            }

            if (txtGoToFrame != null)
            {
                txtGoToFrame.Text = (CurrentFrameIndex + 1).ToString();
            }

            if (chkLoop != null)
            {
                chkLoop.IsChecked = CurrentAnimation.Loop;
            }
        }

        #region Handler

        private void PreviewTextInput_NumberOnly(object sender, TextCompositionEventArgs e)
        {
            //This handler assures that only numbers can be entered into an UI control.
            if (!Char.IsNumber(e.Text[0]))
            {
                e.Handled = true;
            }
        }

        private void slFrames_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Get the value of the slider and check if there's a frame at the position
            //of the slider value in the list.
            int Value = (int)e.NewValue;

            //Value - 1 for the list position
            --Value;
            if (Value < CurrentAnimation.Frames.Count)
            {
                //If frame has been found, set it.
                ChangeFrame(CurrentAnimation.Frames.ElementAt<Frame>(Value));
            }
        }

        private void btnNewFrame_Click(object sender, RoutedEventArgs e)
        {
            AddNewFrame();
        }

        private void btnDeleteFrame_Click(object sender, RoutedEventArgs e)
        {
            DeleteCurrentFrame();
        }

        private void btnAnimation_Click(object sender, RoutedEventArgs e)
        {
            ShowAnimationWindow();
            AnimationWindow.Animation = CurrentAnimation;
            AnimationWindow.StartAnimation();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            AnimationWindow.Close();
        }

        private void txtFrameDuration_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Source = (TextBox)e.Source;
            //Content of the control txtFrameDuration will be set as the duration of the current frame.
            int Duration = Source.Text == "" ? 0 : Convert.ToInt32(Source.Text);
            CurrentAnimation.Frames[CurrentFrameIndex].Duration = Duration;
        }

        private void txtGoToFrame_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Source = (TextBox)e.Source;

            //Get the value of the textbox and check if there's a frame at the position
            //of the textbox's value in the list.
            int Value = Source.Text == "" ? 0 : Convert.ToInt32(Source.Text);

            //Value - 1 for the list position
            --Value;
            if (Value < CurrentAnimation.Frames.Count && Value >= 0)
            {
                //If frame has been found, set it.
                ChangeFrame(CurrentAnimation.Frames.ElementAt<Frame>(Value));
            }
        }

        private void txSpeed_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text[0]) && !(e.Text[0] == ','))
            {
                e.Handled = true;
            }
        }

        private void txSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txSpeed.Text != "")
            {
                AnimationWindow.Speed = Convert.ToDouble(txSpeed.Text);
            }
        }

        private void Click_ImportSprite(object sender, RoutedEventArgs e)
        {
            //This handler opens a new FileDialog, so the user can select an image and add it to
            //the sprite selector.
            System.Windows.Forms.OpenFileDialog FileDialog = new System.Windows.Forms.OpenFileDialog();
            FileDialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";

            if (FileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //Try to load the provided file
                    BitmapImage SpriteBitmap = new BitmapImage(new Uri(FileDialog.FileName, UriKind.Absolute));

                    Image SpriteImage = new Image();
                    
                    SpriteImage.Source = SpriteBitmap;
                    SpriteImage.Width = SpriteBitmap.PixelWidth;
                    SpriteImage.Height = SpriteBitmap.PixelHeight;

                    //Add the sprite source to the animation
                    CurrentAnimation.SpriteSources.Add(new SingleSpriteSource(System.IO.Path.GetFileName(FileDialog.FileName)));

                    //Use the Tag to store the ID of the source
                    SpriteImage.Tag = SpriteSource.GID;

                    //Add the sprite to the sprite selector.
                    AddSprite(SpriteImage);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Click_ClearSprites(object sender, RoutedEventArgs e)
        {
            ClearSprites();
        }

        private void Click_ImportSpriteSheet(object sender, RoutedEventArgs e)
        {
            //This handler opens the SpriteSheetImporter modal.
            SpritesheetImporter Importer = new SpritesheetImporter(this);
            Importer.ShowDialog();
        }

        private void Click_ShowAnimationWindow(object sender, RoutedEventArgs e)
        {
            ShowAnimationWindow();
        }

        private void Click_Quit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Click_NewAnimation(object sender, RoutedEventArgs e)
        {
            CurrentAnimation = new Animation();
            AnimationWindow.Animation = null;
            CurrentFrameIndex = 0;
            UpdateControls();
        }

        private void Click_SaveAnimation(object sender, RoutedEventArgs e)
        {
            CurrentAnimation.Loop = (bool)chkLoop.IsChecked;

            System.Windows.Forms.SaveFileDialog SaveDialog = new System.Windows.Forms.SaveFileDialog();
            SaveDialog.DefaultExt = "xml";
            SaveDialog.Filter = "XML Files(*.xml)|*.xml";
            if (SaveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(Animation));
                TextWriter Writer = new StreamWriter(SaveDialog.FileName);
                Serializer.Serialize(Writer, CurrentAnimation);
                Writer.Close();
            }
        }

        private void Click_LoadAnimation(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog FileDialog = new System.Windows.Forms.OpenFileDialog();
            FileDialog.Filter = "XML Files(*.xml)|*.xml";
            if (FileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(Animation));
                TextReader Reader = new StreamReader(FileDialog.FileName);
                CurrentAnimation = (Animation)Serializer.Deserialize(Reader);
                Reader.Close();
                //TODO

                String ImageFolder = SettingsProperties.ImageFolder;

                //Add sprite sources to selector.
                foreach (SpriteSource s in CurrentAnimation.SpriteSources)
                {
                    if (s is SingleSpriteSource)
                    {
                        Image Sprite = new Image();

                        BitmapImage SpriteBitmap = new BitmapImage(new Uri(ImageFolder + s.Path,UriKind.Absolute));
                        Sprite.Width = SpriteBitmap.PixelWidth;
                        Sprite.Height = SpriteBitmap.PixelHeight;
                        Sprite.Tag = s.ID;

                        Sprite.BeginInit();
                        Sprite.Source = SpriteBitmap;
                        Sprite.EndInit();

                        AddSprite(Sprite);
                    }
                    else
                    {
                        SpriteSheetSource Sheet = (SpriteSheetSource)s;
                        System.Drawing.Bitmap SpriteSheetBitmap = new System.Drawing.Bitmap(ImageFolder + Sheet.Path);

                        Image[] Images = ImageCropper.CropToImageControls(SpriteSheetBitmap, Sheet.SpriteWidth, Sheet.SpriteHeight, Sheet.SpriteAmount);

                        for (int i = 0; i < Sheet.SpriteAmount; ++i)
                        {
                            Images[i].Tag = Sheet.ID + i;
                            AddSprite(Images[i]);
                        }
                    }
                }

                //Add Sprites to Canvas
                foreach (Frame f in CurrentAnimation.Frames)
                {
                    foreach (Sprite s in f.Sprites)
                    {
                        foreach (Border b in spSprites.Children)
                        {
                            Image OriginalImage = (Image)b.Child;
                            int ID = (int)OriginalImage.Tag;

                            if (s.ID == ID)
                            {
                                Image Sprite = new Image();
                                Sprite.BeginInit();
                                Sprite.Source = OriginalImage.Source;
                                Sprite.EndInit();

                                Sprite.Width = OriginalImage.Width;
                                Sprite.Height = OriginalImage.Height;

                                s.Image = Sprite;

                                cvAnimationBoard.Children.Add(Sprite);
                                Canvas.SetLeft(Sprite, s.X + ((cvAnimationBoard.Width * 0.5) - (Sprite.Width * 0.5)));
                                Canvas.SetTop(Sprite, s.Y + ((cvAnimationBoard.Height * 0.5)) - (Sprite.Height * 0.5));
                            }
                        }
                    }
                }

                UpdateControls();
            }
        }

        private void Click_Settings(object sender, RoutedEventArgs e)
        {
            Settings SettingsWindow = new Settings();
            SettingsWindow.ShowDialog();
        }
        #endregion
    } 
}