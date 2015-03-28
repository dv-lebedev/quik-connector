# QuikConnector
-
**Functionality**
- send orders and monitor their execution by channel of orders
- control connection to the terminal
- async functions


```c#
using System;
using QuikConnector.API;
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
                SecuritiesTableName = "SecuritiesTable",
                ServerName = "QServer"
            };

            using (QuikConnector connector = new QuikConnector(parameters))
            {
                connector.Connected += (sender, e) => { Console.WriteLine("Connected."); };
                connector.ImportStarted += (sender, e) => { Console.WriteLine("Import started."); };

                connector.Connect();
                connector.StartImport();

                connector.SecuritiesTable["RIM5"].Updated += RIM5_Updated;

                Console.ReadLine();


                OrderChannel lkoh = connector.CreateOrderChannel("LKOH", "EQBR");

                OrderResult result = lkoh.SendTransaction(Direction.Buy, 3000, 1);

                lkoh.KillOrder(OrderChannel.TransId, result.OrderNumber);
                
                Console.ReadLine();
            }

        }

        static void RIM5_Updated(object sender, Data.Channels.Security e)
        {
            Console.WriteLine("{0}, {1}", e.SecCode, e.PriceOfLastDeal);
        }

    }
}
```
