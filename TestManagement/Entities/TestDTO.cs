using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities.Requests;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class TestDTO : BaseEntity
    {
        public int totalQuestion { get; set; }

        public TestDTO(int id = 0, int totalQuestion = 0) : base(id) { 
            this.totalQuestion = totalQuestion;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{totalQuestion}";
        }

        public static TestDTO ExtractOne(string msg) { 
            TestDTO result = new TestDTO();

            try
            {
                string[] items = msg.Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            result.id = Int32.Parse(items[i]);
                            break;

                        case 1:
                            result.totalQuestion = Int32.Parse(items[i]);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't extract test !!!");
            }

            return result;
        }

        public bool Add(RequestData request) {
            return false;
        }
    }
}
