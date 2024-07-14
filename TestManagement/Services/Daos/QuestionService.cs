using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities;
using TestManagement.Entities.Requests;

namespace TestManagement.Services.Daos
{
    public class QuestionService : DataService
    {
        public List<QuestionDTO> data;

        public QuestionService() : base()
        {
            LoadData();
        }

        public void LoadData()
        {
            data = new List<QuestionDTO>();

            string query = "select id, authorId, quest, correct, " +
                "authorId from tb_question;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines)
                {
                    if (line.Equals("")) continue;
                    data.Add(QuestionDTO.ExtractOne(line));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't load tb_question table !!!");
            }
        }

        public bool Add(RequestData request)
        {
            string query = "insert into tb_question(quest, correct, " +
                "authorId) value(@quest, @correct, @authorId);";

            int result = 0;

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@quest", request.quest);
                cmd.Parameters.AddWithValue("@correct", request.correct);
                cmd.Parameters.AddWithValue("@authorId", request.authorId);

                try
                {
                    connection.Open();

                    result = cmd.ExecuteNonQuery();
                    if (result != 0) Console.WriteLine("Sucessfully add tb_question.");
                    else Console.WriteLine("Failed add tb_question !!!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: Insert tb_question !!!");
                }
                finally
                {
                    connection.Close();
                }

            }

            // add incorrects
            IncorrectService incorrectService = new IncorrectService();
            int questId = 0;
            if (result > 0)
            {
                questId = GetMaxId();
                if (questId != -1)
                {
                    request.questId = questId;
                    foreach (string incorrect in request.incorrects)
                    {
                        request.value = incorrect;
                        if (!incorrectService.Add(request)) return false;
                    }
                }
            }

            return result > 0;
        }

        private int GetMaxId()
        {
            LoadData();
            int maxId = -1;
            foreach (QuestionDTO quest in data)
            {
                maxId = int.Max(maxId, quest.id);
            }
            return maxId;
        }

        public List<QuestionDTO> GenerateQuest(int totalQuestion)
        {
            LoadData();

            List<QuestionDTO> result = new List<QuestionDTO>();

            List<QuestionDTO> temp = data;

            int quantity = int.Min(totalQuestion, data.Count);
            int idx;
            for (int i = 0; i < quantity; i++)
            {
                idx = (new Random()).Next(1, 1000000) % temp.Count;
                result.Add(temp[idx]);
                temp.RemoveAt(idx);
            }

            return result;
        }

        public string GetTestQuestAllStringByTestId(int testId)
        {
            string result = "";

            IncorrectService incorrectService = new IncorrectService();

            List<string> incorrects;

            List<QuestionDTO> quests = GetQuestListByTestId(testId);
            for (int i = 0; i < quests.Count; i++)
            {
                incorrects = incorrectService.GetIncorrectsByQuestId(quests[i].id);

                result += quests[i].id + FormatService.PATTERN_ITEM;
                result += quests[i].authorId + FormatService.PATTERN_ITEM;
                result += quests[i].quest + FormatService.PATTERN_ITEM;
                result += quests[i].Shuffle(incorrects);

                if (i < quests.Count - 1) result += FormatService.PATTERN_END_LINE;
            }

            return result;
        }

        public List<QuestionDTO> GetQuestListByTestId(int testId)
        {
            LoadData();

            TestQuestService testQuestService = new TestQuestService();

            List<QuestionDTO> res = new List<QuestionDTO>();

            foreach (TestQuestDTO testQuest in testQuestService.data)
            {
                if (testQuest.testId == testId)
                {
                    foreach(QuestionDTO question in data)
                    {
                        if(question.id == testQuest.questId)
                        {
                            res.Add(question);
                        }
                    }
                }
            }

            return res;
        }
    }
}
