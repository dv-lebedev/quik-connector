using System;
using System.Collections.Generic;
using System.Linq;


namespace QuikConnector.Orders
{
    public class OrderResult
    {
        public double OrderNumber { get; set; }
        public ReplyCode ReplyCode { get; set; }
        public ResultCode ResultCode { get; set; }
    }
}
