using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestManagement.Entities;

namespace TestManagement.Services.Daos
{
    public class RoleService : DataService
    {
        public List<RoleDTO> data { get; set; }

        public RoleService() : base() {

            LoadRole();
        }

        public void LoadRole() {
            data = new List<RoleDTO>();

            string query = "select id, roleName from tb_role;";

            try
            {
                string dataLine = GetDataLine(GetDataTable(query));
                string[] lines = dataLine.Split(FormatService.PATTERN_END_LINE);
                string[] items;

                foreach (string line in lines) {
                    if (line.Equals("")) continue;
                    data.Add(RoleDTO.ExtractOne(line));
                }
            }
            catch (Exception e) { 
                Console.WriteLine("ERROR: can't load role table !!!");
            }

        }
    }
}
