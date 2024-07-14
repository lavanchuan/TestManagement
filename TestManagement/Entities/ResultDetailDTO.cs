using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class ResultDetailDTO : BaseEntity
    {
        public int resultId { get; set; }
        public int questId { get; set; }
        public string anwser {  get; set; }
        public bool isCorrect { get; set; }

        public ResultDetailDTO(int id = 0, int questId = 0,
            string anwser = "", bool isCorrect = false) : base(id) {

            this.resultId = resultId;
            this.questId = questId;
            this.anwser = anwser;
            this.isCorrect = isCorrect;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{resultId}{FormatService.PATTERN_ITEM}" +
                $"{questId}{FormatService.PATTERN_ITEM}" +
                $"{anwser}{FormatService.PATTERN_ITEM}" +
                $"{isCorrect}";
        }

        public static ResultDetailDTO ExtractOne(string msg)
        {
            ResultDetailDTO result = new ResultDetailDTO();

            try
            {
                string[] items = msg.Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++)
                {
                    switch (i)
                    {
                        case 0: result.id = Int32.Parse(items[i]); break;

                        case 1: result.resultId = Int32.Parse(items[i]); break;

                        case 2: result.questId = Int32.Parse(items[i]); break;

                        case 3: result.anwser = items[i]; break;

                        case 4: result.isCorrect = bool.Parse(items[i]); break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't extract ResultDetailDTO !!!");
            }

            return result;
        }
    }
}
