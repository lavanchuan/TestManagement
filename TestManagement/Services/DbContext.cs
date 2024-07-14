using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities;
using TestManagement.Services.Daos;

namespace TestManagement.Services
{
    public class DbContext
    {
        public RoleService roleService;
        public AccountService accountService;
        public QuestionService questionService;
        public IncorrectService incorrectService;
        public TestService testService;
        public TestQuestService testQuestService;
        public ResultService resultService;
        public ResultDetailService resultDetailService;

        public DbContext()
        {
            Init();
        }

        private void Init()
        {
            roleService = new RoleService();
            accountService = new AccountService();
            questionService = new QuestionService();
            incorrectService = new IncorrectService();
            testService = new TestService();
            testQuestService = new TestQuestService();
            resultService = new ResultService();
            resultDetailService = new ResultDetailService();
        }
    }
}
