using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnimationMaker
{
    /// <summary>
    /// Interaction logic for AnimationWindow.xaml
    /// </summary>
    public partial class AnimationWindow : Window
    {
        public Animation Animation { get; set; }
        public bool Loop { get; set; }
        public double Speed { get; set; }
        public int CurrentFrame { get; set; }
        public int CurrentFrameDuration { get; set; }
        public Timer AnimationTimer { get; set; }

        private System.Threading.Mutex Mutex { get; set; }
        private Point Center { get; set; }

        public AnimationWindow()
        {
            InitializeComponent();
            Speed = 1;
            CurrentFrame = 0;
            Loop = false;
            Animation = null;

            Mutex = new System.Threading.Mutex();
            Center = new Point(this.Width / 2, this.Height / 2);
        }

        public void StartAnimation()
        {
            if (Animation != null)
            {
                AnimationTimer = new Timer();
                ChangeFrame(Animation.Frames[CurrentFrame]);
                AnimationTimer.Interval = 33;
                AnimationTimer.Tick += delegate(object sender, EventArgs e)
                {
                    Frame Frame = Animation.Frames[CurrentFrame];
                    CurrentFrameDuration -= 33;

                    if (CurrentFrameDuration <= 0)
                    {
                        if (++CurrentFrame >= Animation.Frames.Count)
                        {
                            CurrentFrame = 0;

                            if (Loop)
                            {
                                CurrentFrameDuration = Animation.Frames[CurrentFrame].Duration;
                                ChangeFrame(Animation.Frames[CurrentFrame]);

                            }
                            else
                            {
                                AnimationTimer.Stop();
                            }
                        }
                        else
                        {
                            ChangeFrame(Animation.Frames[CurrentFrame]);
                        }
                    }
                };

                AnimationTimer.Start();
            }
        }

        public void ChangeFrame(Frame Frame)
        {
            double ImageCenterX = 0;
            double ImageCenterY = 0;

            Mutex.WaitOne();

            cvAnimationBoard.Children.Clear();
            foreach (Sprite s in Frame.Sprites)
            {
                Image Copy = new Image();
                Copy.BeginInit();
                Copy.Source = s.Image.Source;
                Copy.EndInit();

                Copy.Width = s.Image.Width;
                Copy.Height = s.Image.Height;

                ImageCenterX = Copy.Width / 2;
                ImageCenterY = Copy.Height / 2;

                cvAnimationBoard.Children.Add(Copy);
                Canvas.SetLeft(Copy,s.X + (Center.X - ImageCenterX));
                Canvas.SetTop(Copy,s.Y + (Center.Y - ImageCenterY));
            }

            Mutex.ReleaseMutex();

            CurrentFrameDuration = (int)(Frame.Duration * Speed);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Tag = true;
        }
    }
}
