using System;
using System.Data.SQLite;
using System.Windows;

namespace Database
{
    public class DbCreator
    {
        private SQLiteConnection dbConnection;
        private SQLiteCommand command;
        private string sqlCommand;

        public string createDbConnection()
        {
            string strCon = string.Format("Data Source={0};", "songs.sqlite");
            dbConnection = new SQLiteConnection(strCon);
            dbConnection.Open();
            command = dbConnection.CreateCommand();
            return strCon;
        }

        public void createTable()
        {
            if (!checkIfExist("songs"))
            {
                sqlCommand = "CREATE TABLE songs(name varchar(70), tuning varchar(12), learning bool, learned bool, favorite bool,primary key(name))";
                executeQuery(sqlCommand);
            }
        }

        public bool checkIfExist(string tableName)
        {
            command.CommandText = "SELECT name FROM sqlite_master WHERE name='" + tableName + "'";
            var result = command.ExecuteScalar();

            return result != null && result.ToString() == tableName ? true : false;
        }

        public void executeQuery(string sqlCommand)
        {
            try
            {
                SQLiteCommand triggerCommand = dbConnection.CreateCommand();
                triggerCommand.CommandText = sqlCommand;
                triggerCommand.ExecuteNonQuery();
            }
            catch (Exception)
            {
                MessageBox.Show("Tab with the same name is already added!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool checkIfTableContainsData(string tableName)
        {
            command.CommandText = "SELECT count(*) FROM " + tableName;
            var result = command.ExecuteScalar();

            return Convert.ToInt32(result) > 0 ? true : false;
        }
    }
}