using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Services;

namespace TestManagement.Entities
{
    public class RoleDTO : BaseEntity
    {
        public const string USER = "USER";
        public const string ADMIN = "ADMIN";

        public string roleName { get; set; }

        public RoleDTO(int id = 0, string roleName = USER) : base (id){
            this.roleName = roleName;
        }

        public override string ToString()
        {
            return $"{id}{FormatService.PATTERN_ITEM}{roleName}";
        }

        public static RoleDTO ExtractOne(string msg) { 
            RoleDTO result = new RoleDTO();

            string[] items = msg.Split(FormatService.PATTERN_ITEM);
            for (int i = 0; i < items.Length; i++) {
                switch (i) {
                    case 0: result.id = Int32.Parse(items[i]); break;
                    case 1: result.roleName = items[i]; break;
                    default: break;
                }
            }

            return result;
        }
    }
}
