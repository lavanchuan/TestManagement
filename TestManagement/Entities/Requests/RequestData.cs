using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestManagement.Entities.Requests
{
    public class RequestData
    {
        public int id { get; set; }

        // role
        public string roleName { get; set; }

        // account
        public string accountName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roleId { get; set; }
        public bool isActive { get; set; }

        // question
        public string quest { get; set; }
        public string correct { get; set; }
        public int authorId { get; set; }

        // incorrect
        public string value { get; set; }
        public int questId { get; set; }

        // test
        public int totalQuestion {  get; set; }

        // test_question
        public int testId { get; set; }

        // result
        public int examineeId { get; set; }
        public int correctAmount {  get; set; }

        // result_detail
        public int resultId { get; set; }
        public string anwser {  get; set; }
        public bool isCorrect {  get; set; }

        // other
        public List<string> incorrects { get; set; }
    }
}
