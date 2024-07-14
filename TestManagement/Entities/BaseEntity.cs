using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestManagement.Entities
{
    public class BaseEntity
    {
        public int id { get; set; }

        public BaseEntity(int id = 0) {
            this.id = id;
        }
    }
}
