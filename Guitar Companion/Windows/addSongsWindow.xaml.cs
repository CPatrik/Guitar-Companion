using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Guitar_Companion.Windows
{
    /// <summary>
    /// Interaction logic for addSongsWindow.xaml
    /// </summary>
    public partial class addSongsWindow : Window
    {
        private Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
        private int index = 0;
        List<string> songs = new List<string>();

        public addSongsWindow()
        {
            InitializeComponent();

            fileDialog.Filter = "All Files|*.*|PDF Files|*.pdf|Image Files|*.png,*.jpg|Guitar Pro Files|*.gp*";
            fileDialog.Multiselect = true;

            Nullable<bool> result = fileDialog.ShowDialog();

            if (result == true)
            {
                foreach (string name in fileDialog.FileNames)
                {
                    songs.Add(name);
                }
                FillNextSong();
            }
            else
            {
                Close();
            }
        }

        public addSongsWindow(List<string> songs)
        {
            InitializeComponent();

            this.songs = songs;
            FillNextSong();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine("Tabs", System.IO.Path.GetFileName(songs[index]));
            if (originalFilesCheckBox.IsChecked == true)
            {
                if (!File.Exists(path))
                {
                    File.Copy(songs[index], path);
                }
            }
            else
            {
                if (!File.Exists(path))
                {
                    File.Move(songs[index], path);
                }
            }
            Database.DbCreator db = new Database.DbCreator();
            db.createDbConnection();

            string fileName = System.IO.Path.GetFileName(songs[index]);
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
            if (index < songs.Count)
            {
                songNameTextBlock.Text = songs[index];
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
                    StartInfo = new ProcessStartInfo(songs[index])
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