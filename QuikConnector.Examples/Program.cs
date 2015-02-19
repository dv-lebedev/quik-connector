using System;

namespace QuikConnector.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var quik = new QuikConnection())
            {
                quik.Connected += (s, e) => 
                    { 
                        Console.WriteLine("QUIK is connected."); 
                    };

                quik.Disconnected += (s, e) => 
                    { 
                        Console.WriteLine("QUIK is disconnected."); 
                    };

                quik.Connect();

                AccountParameters account = new AccountParameters
                {
                     Account = "MyAccount",
                      ClientCode = "Test"
                };

                OrderChannel lkoh = new OrderChannel(account, "LKOH", "EQBR");

                lkoh.OrderCallback += (s, e) =>
                {
                    Console.WriteLine("TransId={0}, SecCode={1}, Price={2}, IsSell={3}, Status={4}",
                        e.TransID, e.SecCode, e.Price, e.IsSell, e.Status);
                };

                quik.Subscribe(lkoh);

                //sync transaction
                double orderNum = lkoh.SendTransaction(Direction.Buy, 2000.50M, 10);

                /*          *****        async transaction        ******
                *
                *       15 checks for 100 milliseconds
                *       then -> if the order still not executed - killorder
                */

                lkoh.SendTransactionAsync(Direction.Sell, 2010, 10, 15, 100)
                    .ContinueWith((result) =>
                        {
                            //do something here
                        });


                Console.ReadLine();

            }           
        }//end of Main
    }
}
