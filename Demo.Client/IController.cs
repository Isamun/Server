using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Protocol;

namespace Demo.Client
{
    interface IController : ICommands
    {
        void Connect(String ip, int port);
    }
}
