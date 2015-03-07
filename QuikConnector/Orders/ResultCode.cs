using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Orders
{
    public enum ResultCode 
    {
        Success = 0,
        Failed = 1,
        TerminalNotFound = 2,
        DllVersionNotSupported = 3,
        AlreadyConnectedToQuik = 4,
        WrongSyntax = 5,
        QuikNotConnected = 6,
        DllNotConnected = 7,
        QuikConnected = 8,
        QuikDisconnected = 9,
        DllConnected = 10,
        DllDisconnected = 11,
        MemoryAllocationError = 12,
        WrongConnectionHandle = 13,
        WrongInputParams = 14
    }

}
