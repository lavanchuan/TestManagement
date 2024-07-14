using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class QuestionDTO : BaseEntity
    {
        public int authorId { get; set; }
        public string quest { get; set; }
        public string correctAnwser { get; set; }
        public List<string> incorrectAnwsers { get; set; }

        public QuestionDTO(int id = 0, int authorId = 0, string quest = "", string correctAnwser = "",
            List<string> incorrectAnwsers = null) : base(id)
        {
            this.authorId = authorId;
            this.quest = quest;
            this.correctAnwser = correctAnwser;
            this.incorrectAnwsers = new List<string>();
            if (incorrectAnwsers != null) {
                this.incorrectAnwsers = incorrectAnwsers;
            }
        }

        public override string ToString()
        {
            string pattern = FormatService.PATTERN_ITEM;
            string incorrects = "";
            for (int i = 0; i < incorrectAnwsers.Count; i++) {
                incorrects += incorrectAnwsers[i];
                if (i < incorrectAnwsers.Count - 1) incorrects += pattern;
            }
            return $"{id}{pattern}" +
                $"{authorId}{pattern}" +
                $"{quest}{pattern}" +
                $"{correctAnwser}{pattern}" +
                $"{incorrects}";
        }

        public static QuestionDTO ExtractOne(string msg) {
            QuestionDTO result = new QuestionDTO();
            try
            {
                string[] items = msg.Trim().Split(FormatService.PATTERN_ITEM);
                
                for (int i = 0; i < items.Length; i++) {
                    switch (i)
                    {
                        case 0: result.id = Int32.Parse(items[i]); break;

                        case 1: result.authorId = Int32.Parse(items[i]); break;

                        case 2: result.quest = items[i];break;

                         case 3: result.correctAnwser = items[i]; break;

                        default: result.incorrectAnwsers.Add(items[i]); break;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine($"ERROR: extract {msg} to QuestDTO !!!");
            }

            return result;
        }

        public string Shuffle(List<string> incorrects)
        {
            string res = "";

            List<string> data = new List<string>(incorrects);
            data.Add(correctAnwser);
            int idx = 0;

            while(data.Count > 0)
            {
                idx = ((new Random()).Next(1, 1000000)) % data.Count;
                res += data[idx];
                data.RemoveAt(idx);
                if (data.Count > 0) res += FormatService.PATTERN_ITEM;
            }

            Console.WriteLine(res);

            return res;
        }
    }
}
