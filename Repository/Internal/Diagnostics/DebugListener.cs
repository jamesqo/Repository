using System.Diagnostics;

namespace Repository.Internal.Diagnostics
{
    internal class DebugListener : TraceListener
    {
        public override void Fail(string message, string detailMessage)
        {
            Debugger.Break();
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