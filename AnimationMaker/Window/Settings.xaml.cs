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
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Win32;

namespace AnimationMaker
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog FolderDialog = new FolderBrowserDialog();

            if (FolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFolder.Text = FolderDialog.SelectedPath;
                SettingsProperties.ImageFolder = FolderDialog.SelectedPath + '\\';
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            txtFolder.Text = SettingsProperties.ImageFolder;
        }
    }
}
