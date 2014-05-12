using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Server
{
    interface ITestStandCommands
    {

        /*
         * Play a sequence file
         */
        void ExecuteSequenceFile(
                        String strSeqFilePath, 
                        String strEntryPoint = "MainSequence", 
                        Boolean blocking = true
                    );
    }
}
