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
    public class ResultDetailService : DataService
    {
        public List<ResultDetailDTO> data;

        public ResultDetailService() : base() {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<ResultDetailDTO>();

            string query = "select id, resultId, questId, " +
                "anwser, isCorrect from tb_result_detail;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(ResultDetailDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_result_detail table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_result_detail(resultId, questId, " +
                "anwser, isCorrect) value(@resultId, @questId, " +
                "@anwser, @isCorrect);";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@resultId", request.resultId);
                cmd.Parameters.AddWithValue("@questId", request.questId);
                cmd.Parameters.AddWithValue("@anwser", request.anwser);
                cmd.Parameters.AddWithValue("@isCorrect", request.isCorrect);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_result_detail.");
                    else Console.WriteLine("Failed add tb_result_detail !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_result_detail !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            return result > 0;
        }
    }
}
