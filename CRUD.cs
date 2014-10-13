using System;
using MySql.Data.MySqlClient;

namespace Parallel_Rabin_Karp_Application
{
    public class CRUD
    {
        private MySqlConnection connection;
        private MySqlDataReader dataReader;
        private MySqlCommand cmd;

        private string server;
        private string database;
        private string uid;
        private string password;

        public CRUD()
        {
            server = "localhost";
            database = "nlp";
            uid = "root";
            password = "";

            string connectionString = "SERVER=" + server + ";" +
                                        "USER=" + uid + ";" +
                                        "DATABASE=" + database + ";" +
                                        "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.Number.Equals(0))
                {
                    Console.Write("cannot connect to server. see controller");
                }
                else
                {
                    Console.Write(ex.Message);
                }
            }

            return false;
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }

            return false;
        }

        // PARAMETER
        // qry = is used to inserting query
        // table = is used to choosing column of table
        public string[] Select(string qry, string table_db, string column_db)
        {
            string query = qry;

            int _tableSize = tableSize(query);

            // create an array to store the result
            string[] content = new string[_tableSize];

            // open connection
            if (this.OpenConnection() == true)
            {
                // create command
                cmd = new MySqlCommand(query, connection);

                // create a data reader and execute the command
                dataReader = cmd.ExecuteReader();

                try
                {
                    // read the data and store them onto array
                    int index = 0;
                    while (dataReader.Read())
                    {
                        content[index] = dataReader.GetString(column_db).ToString();
                        index += 1;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.Write(ex.Message);
                }

                // close data reader
                dataReader.Close();

                // close connection
                this.CloseConnection();

                // return list to be displayed
                return content;
            }
            else
            {
                return content;
            }
        }

        private int tableSize(string query)
        {
            int countSize = 0;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;

            if (OpenConnection())
            {

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    countSize += 1;
                }
            }

            CloseConnection();
            return countSize;
        }
    }
}
