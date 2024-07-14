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
    public class TestQuestService : DataService
    {
        public List<TestQuestDTO> data;

        public TestQuestService() : base()
        {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<TestQuestDTO>();

            string query = "select id, testId, questId from tb_test_question;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(TestQuestDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_test_question table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_test_question(testId, questId) value(" +
                "@testId, @questId);";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@testId", request.testId);
                cmd.Parameters.AddWithValue("@questId", request.questId);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_test_question.");
                    else Console.WriteLine("Failed add tb_test_question !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_test_question !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            return result > 0;
        }

        public int GetSumByTestId(int testId)
        {
            LoadData();

            int sum = 0;

            foreach(TestQuestDTO testQuest in data)
            {
                if (testQuest.testId == testId) sum++;
            }

            return sum;
        }
    }
}
