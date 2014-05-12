using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;

namespace Demo.Client
{
    public interface IViewModel
    {

        Execution CurrentExecution {get; set;}
        Boolean Connected {get; set;}



    }
}
