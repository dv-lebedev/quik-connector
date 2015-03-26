using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data
{
    public abstract class DataChannel
    {
        internal virtual bool IsConnected { get; set; }
        protected DateTime DataReceived { get; set; }

        internal bool IsError { get; set; }
        internal void ResetError() { IsError = false; }

        internal void PutDdeData(byte[] data)
        {
            DataReceived = DateTime.UtcNow;

            using (XlTable xt = new XlTable(data))
                ProcessTable(xt);
        }

        protected abstract void ProcessTable(XlTable xt);

    }
}
