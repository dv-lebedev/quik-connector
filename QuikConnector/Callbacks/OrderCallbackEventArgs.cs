using System;

namespace QuikConnector
{
    public class OrderCallbackEventArgs : EventArgs
    {
        public int Mode { get; set; }
        public uint TransID { get; set; }
        public double Number { get; set; }
        public string ClassCode { get; set; }
        public string SecCode { get; set; }
        public double Price { get; set; }
        public int Balance { get; set; }
        public double Value { get; set; }
        public int IsSell { get; set; }
        public int Status { get; set; }
        public int OrderDescriptor { get; set; }
    }
}
