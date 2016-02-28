using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // executes a query and returns the results in a Data Readers
        public MySqlDataReader query(string queryString)
        {
            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = queryString;
            return command.ExecuteReader();
        }

        //executes a non-query and returns the records affected
        public int execute(string queryString)
        {
            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = queryString;
            return command.ExecuteNonQuery();
        }

        // executes an insert and returns the ID of the new record created
        public int executeInsert(string queryString)
        {
            MySqlCommand command = _conn.CreateCommand();
            command.CommandText = queryString;
            command.ExecuteNonQuery();

            // return the new Id
            command.CommandText = "SELECT LAST_INSERT_ID();";
            MySqlDataReader dr = command.ExecuteReader();

            int id = -1;
            if (dr.Read())
            {
                if (dr[0] != DBNull.Value) int.TryParse(dr[0].ToString(), out id);
            }
            dr.Close();
            return id;
        }
    }
}
