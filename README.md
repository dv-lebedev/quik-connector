# QuikConnector
-
**Functionality**
- send orders and monitor their execution by channel of orders
- control connection to the terminal
- async functions


```c#
using System;
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
            var parameters = new ConnectorParameters
            {
                Account = "MyAccount",
                ClientCode = "ClientCode",
                Path = Terminal.GetPathToActiveQuik(),
                ServerName = "QServer"
            };

            using (QConnector connector = new QConnector(parameters))
            {
                connector.Connected += (sender, e) => { Console.WriteLine("Connected."); };

                connector.ImportStarted += (sender, e) => { Console.WriteLine("Import started."); };

                connector.Connect();
                connector.StartImport();


                IDataTable<Security> securitiesTable = connector.AddDataTable<Security>();

                securitiesTable.Updated += securitiesTable_Updated;

                Console.ReadLine();


                OrderChannel lkoh = connector.CreateOrderChannel("LKOH", "EQBR");

                OrderResult result = lkoh.SendTransaction(Direction.Buy, 3000, 1);

                lkoh.KillOrder(OrderChannel.TransId, result.OrderNumber);

                Console.ReadLine();
            }

        }

        static void securitiesTable_Updated(object sender, System.Collections.Generic.List<Security> item)
        {
            var lkoh = item.Find(i => i.Code == "LKOH");

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
