using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class AccountDTO : BaseEntity
    {
        public string accountName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roleId { get; set; }
        public bool isActive { get; set; }

        public AccountDTO(int id = 0, string accountName = "", string username = "",
            string password = "", int roleId = 0, bool isActive = true) : base (id)
        { 
            this.accountName = accountName;
            this.username = username;
            this.password = password;
            this.roleId = roleId;
            this.isActive = isActive;

        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}" +
                $"{accountName}{FormatService.PATTERN_ITEM}" +
                $"{username}{FormatService.PATTERN_ITEM}" +
                $"{password}{FormatService.PATTERN_ITEM}" +
                $"{roleId}{FormatService.PATTERN_ITEM}" +
                $"{isActive}";
        }

        public static AccountDTO ExtractOne(string msg) {
            AccountDTO result = new AccountDTO();

            try
            {
                string[] items = msg.Trim().Split(FormatService.PATTERN_ITEM);
                for (int i = 0; i < items.Length; i++) { 
                    switch(i)
                    {
                        case 0:
                            result.id = Int32.Parse(items[i]);
                            break;

                        case 1:
                            result.accountName = items[i];
                            break;

                        case 2:
                            result.username = items[i];
                            break;

                        case 3:
                            result.password = items[i];
                            break;

                        case 4:
                            result.roleId = Int32.Parse(items[i]);
                            break;

                        case 5:
                            result.isActive = bool.Parse(items[i]);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("ERROR: can't extract account !!!");
            }

            return result;
        }
    }
}
