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
    public class TestService : DataService
    {
        public List<TestDTO> data { get; set; }

        public TestService() : base() {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<TestDTO>();

            string query = "select id, totalQuestion from tb_test;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(TestDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_test table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_test(totalQuestion) value(@totalQuestion);";

            int result = 0;

            if (request.totalQuestion == 0) request.totalQuestion = 1;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@totalQuestion", request.totalQuestion);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_test.");
                    else Console.WriteLine("Failed add tb_test !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_test !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            // generate test_quest
            QuestionService questionService = new QuestionService();
            TestQuestService testQuestService = new TestQuestService();
            int testId = GetMaxId();
            List<QuestionDTO> quests = questionService.GenerateQuest(request.totalQuestion);

            ///// UPDATE totalQuestion in tb_test //TODO

            for(int i = 0; i < quests.Count; i++)
            {
                request.testId = testId;
                request.questId = quests[i].id;
                if (!testQuestService.Add(request)) return false;
            }

            return result > 0;

        }

        private int GetMaxId()
        {
            LoadData();
            int maxId = -1;
            foreach (TestDTO test in data) {
                maxId = int.Max(maxId, test.id);
            }
            return maxId;
        }

        public bool Update(RequestData data) {

            int result = 0;

            return result > 0;
        }

        public bool ExistsById(int testId)
        {
            LoadData();
            foreach (TestDTO test in data) if (test.id == testId) return true;
            return false;
        }
    }
}
