using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuikConnector.Data.Channels
{
    public class Security
    {
        public string ShortName { get; set; }
        public string Code { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
        public decimal Bid { get; set; }
        public decimal BidVolume { get; set; }
        public decimal Ask { get; set; }
        public decimal AskVolume { get; set; }
        public decimal Price { get; set; }
        public decimal Lot { get; set; }
    }
}
