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
using System.IO;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;

namespace Guitar_Companion.Windows
{
    /// <summary>
    /// Interaction logic for addSongsWindow.xaml
    /// </summary>
    public partial class addSongsWindow : Window
    {
         Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
         int index = 0;

        public addSongsWindow()
        {
            InitializeComponent();

            fileDialog.DefaultExt = ".pdf";
            fileDialog.Filter = "PDF Files (*.pdf)|*.pdf|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|GP5 Files (*.gp5)|*.gp5|GPX Files (*.gpx)|*.gpx";
            fileDialog.Multiselect = true;

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                FillNextSong();
            }
            else
            {
                Close();
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine("Tabs", System.IO.Path.GetFileName(fileDialog.FileNames[index]));
            if (originalFilesCheckBox.IsChecked == true)
            {
                if (!File.Exists(path))
                {
                    File.Copy(fileDialog.FileNames[index], path);
                }
            }
            else
            {
                if (!File.Exists(path))
                {
                    File.Move(fileDialog.FileNames[index], path);
                }
            }
            Database.DbCreator db = new Database.DbCreator();
            db.createDbConnection();

            string fileName = System.IO.Path.GetFileName(fileDialog.FileNames[index]);
            string tuning = "";

            if (tuningComboBox.SelectedIndex == 0)
            {
                tuning = "EADGBE";
            }
            else if (tuningComboBox.SelectedIndex == 1)
            {
                tuning = "DADGBE";
            }
            else if (tuningComboBox.SelectedIndex == 2)
            {
                tuning = "FADGBE";
            }
            else if (tuningComboBox.SelectedIndex == 3)
            {
                if (customTextBox.Text == "Custom Tuning")
                {
                    MessageBox.Show("Custom tuning not set");
                    this.Close();
                }
                else
                {
                    tuning = customTextBox.Text;
                }
            }
            db.executeQuery($"insert into songs values(\"{fileName}\",\"{tuning}\",false,false,false)");
            tuningComboBox.SelectedIndex = 0;
            customTextBox.Text = "Custom Tuning";
            index++;
            FillNextSong();
        }

        private void FillNextSong()
        {
            if (index<fileDialog.FileNames.Length)
            {
                songNameTextBlock.Text = fileDialog.FileNames[index];
            }
            else
            {
                this.Close();
            }
        }

        private void tuningComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tuningComboBox.SelectedIndex == 3)
            {
                customTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                if (customTextBox != null)
                {
                    customTextBox.Visibility = Visibility.Hidden;
                }
            }
        }

        private void skipButton_Click(object sender, RoutedEventArgs e)
        {
            index++;
            FillNextSong();
        }

        private void openOriginalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(fileDialog.FileNames[index])
                    {
                        UseShellExecute = true
                    }
                }.Start();

            }
            catch (Exception)
            {

                MessageBox.Show("Error opening the original tab!");
            }
        }
    }
}
