using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;
using Demo.Net;

namespace Demo.Server
{
    public class DemoObjectGen
    {
        public Procedure MakeAProcedure(object content)
        {
            Procedure pro = new Procedure(false)
            {
                Name = "First demo procedure",
                Date = DateTime.Now,
                Description = "FAT testing of XT. Contains 3 executions"
                

            };
            return pro;
        }
    }
}
