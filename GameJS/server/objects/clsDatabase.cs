using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace GameJS
{
    public class clsDatabase
    {
        // map objects
        private string _database;
        private string _password;
        private MySqlConnection _conn;
        

        public clsDatabase(string database, string password)
        {
            _database = database;
            _password = password;

            string myConnectionString = "server=127.0.0.1;uid=root;pwd=" + this.password + ";database=" + this.database + ";";
            _conn = new MySqlConnection(myConnectionString);
        }

        public string database
        {
            get { return _database; }
        }

        public string password
        {
            get { return _password; }
        }

        public MySqlConnection conn
        {
            get { return _conn; }
        }

        public void open()
        {
            if (_conn != null && _conn.State == ConnectionState.Closed)
            {
                _conn.Open();
            }
        }

        public void close()
        {
            if (_conn != null && _conn.State != ConnectionState.Closed)
            {
                _conn.Close();
            }
        }

        public MySqlDataReader query(string queryString)
        {
            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = queryString;
            return command.ExecuteReader();
        }

        public int execute(string queryString)
        {
            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = queryString;
            return command.ExecuteNonQuery();
        }
    }
}
