using System;

namespace QuikConnector
{
    public class TradeCallbackEventArgs : EventArgs
    {
        public int Mode { get; set; }
        public double Number { get; set; }
        public double OrderNumber { get; set; }
        public string ClassCode { get; set; }
        public string SecCode { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double Value { get; set; }
        public int IsSell { get; set; }
        public int TradeDescription { get; set; }
    }
}
