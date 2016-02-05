/*
The MIT License (MIT)

Copyright (c) 2015 Denis Lebedev

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

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
