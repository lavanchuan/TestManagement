using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class ResultDTO : BaseEntity
    {
        public int testId {  get; set; }
        public int examineeId { get; set; }
        public int correctAmount { get; set; }
        public int totalQuestion {  get; set; }

        public ResultDTO(int id = 0, int testId = 0, int examineeId = 0,
            int correctAmount = 0, int totalQuestion = 0) : base(id) {
            this.testId = testId;
            this.examineeId = examineeId;
            this.correctAmount = correctAmount;
            this.totalQuestion = totalQuestion;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{testId}{FormatService.PATTERN_ITEM}" +
                $"{examineeId}{FormatService.PATTERN_ITEM}" +
                $"{correctAmount}{FormatService.PATTERN_ITEM}" +
                $"{totalQuestion}";
        }

        public static ResultDTO ExtractOne(string msg)
        {
            ResultDTO result = new ResultDTO();

            try
            {
                string[] items = msg.Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++)
                {
                    switch (i)
                    {
                        case 0: result.id = Int32.Parse(items[i]); break;

                        case 1: result.testId = Int32.Parse(items[i]); break;

                        case 2: result.examineeId = Int32.Parse(items[i]); break;

                        case 3: result.correctAmount = Int32.Parse(items[i]); break;

                        case 4: result.totalQuestion = Int32.Parse(items[i]); break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't extract ResultDTO !!!");
            }

            return result;
        }
    }
}
