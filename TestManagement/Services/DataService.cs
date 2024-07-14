using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestManagement.Services
{
    public class DataService
    {
        public string server { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password { get; set; }

        public MySqlConnection connection { get; set; }

        public DataService(string server = "LOCALHOST", string database = "db_test_management",
            string username = "root", string password = "") {
            this.server = server;
            this.database = database;
            this.username = username;
            this.password = password;

            Connect();
        }

        private void Connect() {
            try
            {
                connection = new MySqlConnection(ConnectionString());
            }
            catch (Exception e) { 
                Console.WriteLine("Can't connect to database !!!");
            }
        }

        private string ConnectionString() {
            return "SERVER=" + server + " ;" +
                "DATABASE=" + database + " ;" +
                "UID=" + username + " ;" +
                "PASSWORD=" + password + " ;";
        }

        public DataTable GetDataTable(string query)
        {
            try
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);

                DataTable dataTable = new DataTable();

                connection.Open();

                adapter.Fill(dataTable);

                connection.Close();

                return dataTable;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't get data from database !!!");

                throw new Exception();
            }
        }


        public string GetDataLine(DataTable dataTable)
        {
            string datas = "";
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        datas += item + FormatService.PATTERN_ITEM;
                    }
                    datas += FormatService.PATTERN_END_LINE;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR can't get data line !!!");
                throw new Exception();
            }
            return datas;
        }
    }
}
