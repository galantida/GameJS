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
        private MySqlConnection _conn;
        

        public clsDatabase(string conString)
        {
            _conn = new MySqlConnection(conString);
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
