using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal class DebugListener : TraceListener
    {
        public override void Fail(string message, string detailMessage)
        {
            throw new DebugAssertException(message);
        }

        public override void Write(string message)
        {
            // The default trace listener already writes to the Output window.
        }

        public override void WriteLine(string message)
        {
            // The default trace listener already writes to the Output window.
        }
    }
}