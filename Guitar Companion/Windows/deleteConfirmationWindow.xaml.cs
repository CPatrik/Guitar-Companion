using System.IO;
using System.Windows;
using Path = System.IO.Path;

namespace Guitar_Companion.Windows
{
    /// <summary>
    /// Interaction logic for deleteConfirmationWindow.xaml
    /// </summary>
    public partial class deleteConfirmationWindow : Window
    {
        public deleteConfirmationWindow()
        {
            InitializeComponent();
            if (MainWindow.deleteSongs.Count == 1)
            {
                itemsTextBlock.Text = Path.GetFileName(MainWindow.deleteSongs[0]);
            }
            else if (MainWindow.deleteSongs.Count > 1)
            {
                for (int i = 0; i < MainWindow.deleteSongs.Count; i++)
                {
                    itemsTextBlock.Text += Path.GetFileName(MainWindow.deleteSongs[i]) + "\n";
                }
            }
            else
            {
                this.Close();
            }
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.deleteSongs.Count == 1)
            {
                Database.DbCreator db = new Database.DbCreator();
                db.createDbConnection();
                db.executeQuery($"delete from songs where name=\"{Path.GetFileName(MainWindow.deleteSongs[0])}\"");
                File.Delete(MainWindow.deleteSongs[0]);

                this.Close();
            }
            else if (MainWindow.deleteSongs.Count > 1)
            {
                for (int i = 0; i < MainWindow.deleteSongs.Count; i++)
                {
                    Database.DbCreator db = new Database.DbCreator();
                    db.createDbConnection();
                    db.executeQuery($"delete from songs where name=\"{Path.GetFileName(MainWindow.deleteSongs[i])}\"");
                    File.Delete(MainWindow.deleteSongs[i]);

                    this.Close();
                }
            }
            else if (MainWindow.deleteSongs.Count < 1)
            {
                this.Close();
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}