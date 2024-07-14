using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class IncorrectDTO : BaseEntity
    {
        public string value {  get; set; }
        public int questId { get; set; }

        public IncorrectDTO(int id = 0, string value = "", int questId = 0) : base(id) {
            this.value = value;
            this.questId = questId;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{value}{FormatService.PATTERN_ITEM}" +
                $"{questId}";
        }

        public static IncorrectDTO ExtractOne(string msg)
        {
            IncorrectDTO result = new IncorrectDTO();

            try
            {
                string[] items = msg.Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++)
                {
                    switch (i)
                    {
                        case 0: result.id = Int32.Parse(items[i]); break;

                        case 1: result.value = items[i]; break;

                        case 2: result.questId = Int32.Parse(items[i]); break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: can't extract incorrect !!!");
            }

            return result;
        }
    }
}
