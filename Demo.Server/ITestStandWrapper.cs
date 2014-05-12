using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;

namespace Demo.Server
{
    public interface ITestStandWrapper : ICommands, IProtocolEvents
    {
        void init();
        
    }
}
