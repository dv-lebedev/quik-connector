using System;

namespace QuikConnector
{
    public class ConnectionStatusEventArgs : EventArgs
    {
        public UInt32 nConnectionEvent { get; set; }
        public UInt32 nExtendedErrorCode { get; set; }
        public byte[] lpstrInfoMessage { get; set; }

        public override string ToString()
        {
            return QuikApi.ResultToString(nConnectionEvent);
        }
    }
}
