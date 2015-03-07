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


                OrderChannel lkoh = new OrderChannel("SPBFUT00902","Test", "RIH5", "SPBFUT");

                lkoh.OrderCallback += (s, e) =>
                {
                    Console.WriteLine("CALLBACK: TransId={0}, SecCode={1}, Price={2}, IsSell={3}, Status={4}",
                        e.TransID, e.SecCode, e.Price, e.IsSell, e.Status);
                };

                quik.Subscribe(lkoh);

                Orders.OrderResult result = lkoh.SendTransaction(Direction.Buy, 91000.00M, 1);

                Console.WriteLine(result.ReplyCode);
                Console.WriteLine(result.ResultCode);
               // Console.Read();

                
                Console.WriteLine("connect=" + quik.IsConnected);
               //double reply =  lkoh.KillOrder(OrderChannel.TransId, num);

               //Console.WriteLine("Reply Code=" + reply);
                Console.Read();
                quik.Disconnect();
                Console.ReadLine();

            }           
        }//end of Main
    }
}
