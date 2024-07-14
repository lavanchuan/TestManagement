using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities;
using TestManagement.Entities.Requests;

namespace TestManagement.Services.Daos
{
    public class AccountService : DataService
    {
        public List<AccountDTO> data { get; set;}

        public AccountService() {

            LoadData();
        }

        public void LoadData() {
            data = new List<AccountDTO>();

            string query = "select id, accountName, username," +
                " password, roleId, isActive from tb_account;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(AccountDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_account table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_account(accountName, username, password, " +
                "roleId, isActive) value(@accountName, @username, @password, 1, true)";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@accountName", request.accountName);
                cmd.Parameters.AddWithValue("@username", request.username);
                cmd.Parameters.AddWithValue("@password", request.password);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add account.");
                    else Console.WriteLine("Failed add account !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_account !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            return result > 0;
        }

        public AccountDTO GetById(int accountId)
        {
            LoadData();

            foreach (AccountDTO acc in data) {
                if (acc.id == accountId) return acc;
            }

            return null;
        }

        public int Authentication(RequestData request)
        {
            LoadData();

            foreach (AccountDTO acc in data) { 
                if(request.username.Equals(acc.username) &&
                    request.password.Equals(acc.password)) return acc.id;
            }

            return -1;
        }

        public bool ExistsById(int authorId)
        {
            LoadData();
            foreach (AccountDTO acc in data)
            {
                if (acc.id == authorId) return true;
            }

            return false;
        }

        public bool ExistsByUsername(string username)
        {
            LoadData();
            foreach (AccountDTO acc in data) if (acc.username.Equals(username)) return true;
            return false;
        }

        public AccountDTO GetByUsername(string username)
        {
            LoadData();
            foreach (AccountDTO acc in data) if (acc.username.Equals(username)) return acc;
            return null;
        }
    }
}
