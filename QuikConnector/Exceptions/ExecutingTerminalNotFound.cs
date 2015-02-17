using System;

namespace QuikConnector
{
    public class ExecutingTerminalNotFound : Exception
    {
        public override string Message
        {
            get
            {
                return "Executing terminal is not found. Make sure that the terminal is launched.";
            }
        }
    }
}
