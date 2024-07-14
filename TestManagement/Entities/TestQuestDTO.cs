using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class TestQuestDTO : BaseEntity
    {
        public int testId {  get; set; }
        public int questId { get; set; }

        public TestQuestDTO(int id = 0, int testId = 0, int questId = 0) : base(id) {
            this.testId = testId;
            this.questId = questId;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{testId}{FormatService.PATTERN_ITEM}" +
                $"{questId}";
        }

        public static TestQuestDTO ExtractOne(string msg)
        {
            TestQuestDTO result = new TestQuestDTO();

            try
            {
                string[] items = msg.Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++)
                {
                    switch (i)
                    {
                        case 0: result.id = Int32.Parse(items[i]); break;

                        case 1: result.testId = Int32.Parse(items[i]); break;

                        case 2: result.questId = Int32.Parse(items[i]); break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't extract Test_QuestDTO !!!");
            }

            return result;
        }
    }
}
