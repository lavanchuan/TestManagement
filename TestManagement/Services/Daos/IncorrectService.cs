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
    public class IncorrectService : DataService
    {
        public List<IncorrectDTO> data;

        public IncorrectService() : base()
        {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<IncorrectDTO>();

            string query = "select id, value, questId from tb_incorrect;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(IncorrectDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_incorrect table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_incorrect(value, questId) value(" +
                "@value, @questId);";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@value", request.value);
                cmd.Parameters.AddWithValue("@questId", request.questId);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_incorrect.");
                    else Console.WriteLine("Failed add tb_incorrect !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_incorrect !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            return result > 0;
        }

        internal List<string> GetIncorrectsByQuestId(int questId)
        {
            LoadData();
            List<string> res = new List<string>();

            foreach(IncorrectDTO incorrect in data)
            {
                if (incorrect.questId == questId) res.Add(incorrect.value);
            }

            return res;
        }
    }
}
