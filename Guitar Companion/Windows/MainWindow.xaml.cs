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
using System.Data.SQLite;
using System.IO;
using System.Data;
using System.Diagnostics;
using Path = System.IO.Path;

namespace Guitar_Companion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> deleteSongs = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("songs.sqlite"))
            {
                SQLiteConnection.CreateFile("songs.sqlite");
                Database.DbCreator db = new Database.DbCreator();
                db.createDbConnection();
                db.createTable();
            }
            if (!Directory.Exists("Tabs"))
            {
                Directory.CreateDirectory("Tabs");
            }

            DataBaseConnection("SELECT * FROM songs");
        }

        private void addTabsButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.addSongsWindow addSongsWindow = new Windows.addSongsWindow();
            this.Show();
            addSongsWindow.Show();
            this.Show();
        }
        public void DataBaseConnection(string query)
        {
            try
            {
                SQLiteConnection sqlite = new SQLiteConnection("Data Source=songs.sqlite; Version=3;");

                SQLiteCommand sqlcmd;
                sqlite.Open();
                sqlcmd = sqlite.CreateCommand();
                sqlcmd.CommandText = query;
                SQLiteDataAdapter sda = new SQLiteDataAdapter(sqlcmd);
                DataTable dt = new DataTable("Songs");
                sda.Fill(dt);
                
                tabsDataGrid.ItemsSource = dt.DefaultView;
                sqlite.Close();

                FillTabsLoaded(tabsDataGrid.Items.Count);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("SQLite DataBase Error!");
            }

        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTabs();
        }

        private void OpenTabs()
        {
            DataRowView dataRowView;
            string path;
            try
            {
                if (tabsDataGrid.SelectedItems.Count > 1)
                {
                    for (int i = 0; i < tabsDataGrid.SelectedItems.Count; i++)
                    {
                        dataRowView = (DataRowView)tabsDataGrid.SelectedItems[i];
                        path = AppDomain.CurrentDomain.BaseDirectory + @"Tabs/" + dataRowView.Row[0].ToString();

                        new Process
                        {
                            StartInfo = new ProcessStartInfo(path)
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                    }
                }
                else if (tabsDataGrid.SelectedItems.Count == 1)
                {
                    dataRowView = (DataRowView)tabsDataGrid.SelectedItem;
                    path = AppDomain.CurrentDomain.BaseDirectory + @"Tabs/" + dataRowView.Row[0].ToString();

                    new Process
                    {
                        StartInfo = new ProcessStartInfo(path)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
                searchTextBox.Text = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Error while loading the tab!");
            }
            
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView;
            string path;
            if (tabsDataGrid.SelectedItems.Count == 1)
            {
                MainWindow.deleteSongs.Clear();
                dataRowView = (DataRowView)tabsDataGrid.SelectedItem;
                path = AppDomain.CurrentDomain.BaseDirectory + @"Tabs/" + dataRowView.Row[0].ToString();
                MainWindow.deleteSongs.Add(path);

                Windows.deleteConfirmationWindow deleteConfirmationWindow = new Windows.deleteConfirmationWindow();
                this.Show();
                deleteConfirmationWindow.Show();
                this.Show();
                DataBaseConnection("SELECT * FROM songs");
            }
            else if (tabsDataGrid.SelectedItems.Count > 1)
            {
                MainWindow.deleteSongs.Clear();
                for (int i = 0; i < tabsDataGrid.SelectedItems.Count; i++)
                {
                    dataRowView = (DataRowView)tabsDataGrid.SelectedItems[i];
                    path = AppDomain.CurrentDomain.BaseDirectory + @"Tabs/" + dataRowView.Row[0].ToString();
                    MainWindow.deleteSongs.Add(path);
                }

                Windows.deleteConfirmationWindow deleteConfirmationWindow = new Windows.deleteConfirmationWindow();
                this.Show();
                deleteConfirmationWindow.Show();
                this.Show();
                DataBaseConnection("SELECT * FROM songs");
                searchTextBox.Text = "";
            }
            else
            {
                MessageBox.Show("Select a song or songs from the list to delete!");
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            DataBaseConnection("SELECT * FROM songs");
            searchTextBox.Text = "";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataBaseConnection("SELECT * FROM songs");
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text != "")
            {
                DataBaseConnection($"select * from songs where name like \'%{searchTextBox.Text}%\'");
            }
        }

        private void tabsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenTabs();
        }

        private void tabsDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (tabsDataGrid.ActualWidth > 689)
            {
                int width = (int)(tabsDataGrid.ActualWidth - 300);
                datagridName.MinWidth = width;
            }
            else
            {
                datagridName.MinWidth = 386;
            }
        }

        private void addToFavorites_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(tabsDataGrid.SelectedItems.Count > 1))
                {
                    string name;
                    bool favorite;
                    DataRowView dataRowView = (DataRowView)tabsDataGrid.SelectedItem;
                    name = dataRowView.Row[0].ToString();
                    favorite = (bool)dataRowView.Row[4];
                    if (favorite)
                    {
                        DataBaseConnection($"update songs set favorite=false where name=\"{name}\"");
                        DataBaseConnection("SELECT * FROM songs");
                    }
                    else
                    {
                        DataBaseConnection($"update songs set favorite=true where name=\"{name}\"");
                        DataBaseConnection("SELECT * FROM songs");
                    }
                }
                else
                {
                    MessageBox.Show("Please only select 1 tab!");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Tab not selected!");
            }
            
            
        }

        private void learningToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(tabsDataGrid.SelectedItems.Count > 1))
                {
                        string name;
                        bool learning;
                        DataRowView dataRowView = (DataRowView)tabsDataGrid.SelectedItem;
                        name = dataRowView.Row[0].ToString();
                        learning = (bool)dataRowView.Row[2];
                        if (learning)
                        {
                            DataBaseConnection($"update songs set learning=false where name=\"{name}\"");
                            DataBaseConnection("SELECT * FROM songs");
                        }
                        else
                        {
                            DataBaseConnection($"update songs set learning=true where name=\"{name}\"");
                            DataBaseConnection("SELECT * FROM songs");
                        }
                }
                else
                {
                    MessageBox.Show("Please only select 1 tab!");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tab not selected!");
            }
            
        }

        private void learnedToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(tabsDataGrid.SelectedItems.Count > 1))
                {
                    string name;
                    bool learned;
                    DataRowView dataRowView = (DataRowView)tabsDataGrid.SelectedItem;
                    name = dataRowView.Row[0].ToString();
                    learned = (bool)dataRowView.Row[3];
                    if (learned)
                    {
                        DataBaseConnection($"update songs set learned=false where name=\"{name}\"");
                        DataBaseConnection("SELECT * FROM songs");
                    }
                    else
                    {
                        DataBaseConnection($"update songs set learned=true where name=\"{name}\"");
                        DataBaseConnection("SELECT * FROM songs");
                    }
                }
                else
                {
                    MessageBox.Show("Please only select 1 tab!");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Tab not selected!");
            }
            
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
            bool isFavorite = FavoritCheckBox.IsChecked.Value;
            bool isLearning = learningCheckBox.IsChecked.Value;
            bool isLearned = learnedCheckBox.IsChecked.Value;

            string command = "SELECT * from songs where name IS NOT NULL";

            if (tuningComboBox.SelectedIndex == 0)
            {
                
            }
            else if (tuningComboBox.SelectedIndex == 4)
            {
                command+= " AND tuning!=\"EADGBE\" AND tuning!=\"DADGBE\" AND tuning!=\"FADGBE\"";
            }
            else
            {
                if (tuningComboBox.SelectedIndex == 1)
                {
                    command+= " AND tuning=\"EADGBE\"";
                }
                else if (tuningComboBox.SelectedIndex == 2)
                {
                    command += " AND tuning=\"DADGBE\"";
                }
                else
                {
                    command += " AND tuning=\"FADGBE\"";
                }
            }
            if (isFavorite)
            {
                command += " AND favorite=true";
            }
            if (isLearning)
            {
                command += " AND learning=true";
            }
            if (isLearned)
            {
                command += " AND learned=true";
            }
            try
            {
                DataBaseConnection(command);
            }
            catch (Exception)
            {

                MessageBox.Show("Invalid sql query");
            }
            


        }

        private void tuningComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tuningComboBox.SelectedIndex == 0)
            {
                DataBaseConnection("SELECT * FROM songs");
            }
            else if (tuningComboBox.SelectedIndex == 4)
            {
                DataBaseConnection("SELECT * FROM songs where tuning!=\"EADGBE\" AND tuning!=\"DADGBE\" AND tuning!=\"FADGBE\"");
            }
            else
            {
                string tuning;
                if (tuningComboBox.SelectedIndex == 1)
                {
                    tuning = "EADGBE";
                }
                else if (tuningComboBox.SelectedIndex == 2)
                {
                    tuning = "DADGBE";
                }
                else
                {
                    tuning = "FADGBE";
                }
                DataBaseConnection($"SELECT * FROM songs where tuning=\"{tuning}\"");
            }

        }

        private void openRandomTab_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            tabsDataGrid.SelectedIndex = rnd.Next(0,tabsDataGrid.Items.Count);
            OpenTabs();
            tabsDataGrid.Focus();
        }

        private void FillTabsLoaded(int count)
        {
            tabsLoadedLabel.Text = (count-1) + " tabs loaded";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch (Exception)
            {

                Console.WriteLine("Error moving the window");
            }

        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void fullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState=WindowState.Normal;
            }
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
