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
    public class ResultService : DataService
    {
        public List<ResultDTO> data;

        public ResultService() : base()
        {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<ResultDTO>();

            string query = "select id, testId, examineeId, " +
                "correctAmount, totalQuestion from tb_result";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                Console.WriteLine("[Checked]");

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(ResultDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_result table !!!");
                throw new Exception();
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_result(testId, examineeId, " +
                "correctAmount, totalQuestion) value(@testId, @examineeId, " +
                "@correctAmount, @totalQuestion);";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@testId", request.testId);
                cmd.Parameters.AddWithValue("@examineeId", request.examineeId);
                cmd.Parameters.AddWithValue("@correctAmount", request.correctAmount);
                cmd.Parameters.AddWithValue("@totalQuestion", request.totalQuestion);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_result.");
                    else Console.WriteLine("Failed add tb_result !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_result !!!");
                    throw new Exception();
                }
                finally
                {
                    connection.Close();
                }

            }

            return result > 0;
        }

        public int GetMaxId()
        {
            LoadData();
            int maxId = -1;
            foreach(ResultDTO result in data)
            {
                maxId = int.Max(maxId, result.id);
            }

            return maxId;
        }
    }
}
