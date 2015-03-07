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


                OrderChannel lkoh = new OrderChannel("ACCOUNT","Test", "LKOH", "EQBR");

                lkoh.OrderCallback += (s, e) =>
                {
                    Console.WriteLine("CALLBACK: TransId={0}, SecCode={1}, Price={2}, IsSell={3}, Status={4}",
                        e.TransID, e.SecCode, e.Price, e.IsSell, e.Status);
                };

                quik.Subscribe(lkoh);

                Orders.OrderResult result = lkoh.SendTransaction(Direction.Buy, 3000, 1);

                Console.WriteLine("ReplyCode = {0}", result.ReplyCode);
                Console.WriteLine("ResultCode = {0}", result.ResultCode);
               // Console.Read();

                
                lkoh.KillOrder(OrderChannel.TransId, num);


                Console.Read();
                quik.Disconnect();
                Console.ReadLine();

            }           
        }//end of Main
    }
}
