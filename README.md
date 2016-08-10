# QuikConnector

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.Net version](https://img.shields.io/badge/.NET%20Framework-v4.6.1-brightgreen.svg)

Quik settings in 'info.wnd'

##Functionality
- send orders and monitor their execution by channel of orders
- get real-time market data
- control connection to Quik
- async functions


```c#
using System;
using System.Collections.Generic;
using QuikConnector.API;
using QuikConnector.Core;
using QuikConnector.Data;
using QuikConnector.Orders;

namespace QuikConnector.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            var parameters = new ConnectorParameters(
                account: "MyAccount",
                pathToQuik: Terminal.GetPathToActiveQuik(),
                ddeServerName: "QServer");


            using (var QUIK = new QConnector(parameters))
            {
                QUIK.Connection.Connected += (sender, e) => { Console.WriteLine("Connected."); };

                QUIK.ImportStarted += (sender, e) => { Console.WriteLine("Import started."); };

                QUIK.Connect();
                QUIK.StartImport();


                IDataTable<Security> securitiesTable = QUIK.AddDataTable<Security>();

                securitiesTable.Updated += securitiesTable_Updated;

                Console.ReadLine();


                OrderChannel lkoh = QUIK.CreateOrderChannel("LKOH", "EQBR");

                OrderResult result = lkoh.SendTransaction(Direction.Buy, price: 3000.00M, volume: 1);
                
                lkoh.KillOrder(result.TransId, result.OrderNumber);

                Console.ReadLine();
            }

        }

        static void securitiesTable_Updated(object sender, List<Security> e)
        {
            var lkoh = e.Find(i => i.Code == "LKOH");

            if (lkoh != null)
            {
                Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}",
                    lkoh.ShortName, lkoh.Code, lkoh.Class, lkoh.Bid, lkoh.Ask, lkoh.Price);
            }
        }

    }

    [QuikTable(Name="SecuritiesTable")]
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


```

##Dependencies
- NDde

## License
[MIT](LICENSE)
