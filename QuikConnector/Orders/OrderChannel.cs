using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using QuikConnector.Orders;

namespace QuikConnector
{
    [Serializable]
    public class OrderChannel 
    {
        public static long TransId { get; private set; }

        public string Account { get; private set; }
        public string ClientCode { get; set; }

        public string SecCode { get; set; }
        public string ClassCode { get; set; }

        public SortedDictionary<double, OrderCallbackEventArgs> Orders { get; protected set; }
        public SortedDictionary<double, TradeCallbackEventArgs> Trades { get; protected set; }


        public event EventHandler<OrderCallbackEventArgs> OrderCallback;

        public event EventHandler<TradeCallbackEventArgs> TradeCallback;


        protected OrderChannel()
        {
            Orders = new SortedDictionary<double, OrderCallbackEventArgs>();
            Trades = new SortedDictionary<double, TradeCallbackEventArgs>();
        }

        public OrderChannel(string account, string clientCode,
            string secCode, string classCode)
            : this()
        {
            Account = account;
            ClientCode = clientCode;
            SecCode = secCode;
            ClassCode = classCode;
        }


        public void OnOrderCallback(OrderCallbackEventArgs e)
        {
            if (Orders.ContainsKey(e.Number))
            {
                Orders[e.Number] = e;
            }
            else
            {
                Orders.Add(e.Number, e);
            }

            if (OrderCallback != null) OrderCallback(this, e);
        }

        public void OnTradeCallback(TradeCallbackEventArgs e)
        {
            if (Trades.ContainsKey(e.Number))
            {
                Trades[e.Number] = e;
            }
            else
            {
                Trades.Add(e.Number, e);
            }

            if (TradeCallback != null) TradeCallback(this, e);
        }


        public OrderResult SendTransaction(Direction direction, decimal price, int volume, string clientcode)
        {
            TransId++;

            string transactionString = string.Format(CultureInfo.InvariantCulture, "ACCOUNT={0};TYPE=L;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE={5};QUANTITY={6};CLIENT_CODE={7};",
                Account, TransId, ClassCode, SecCode, (char)direction, price, volume, clientcode);

            double orderNum = 0;

            long replyCode = 0;

            long result = QuikApi.send_sync_transaction_test(transactionString, ref orderNum, ref replyCode);

            return new OrderResult
            {
                OrderNumber = orderNum,
                ReplyCode = (ReplyCode)replyCode,
                ResultCode = (ResultCode)result
            };
        }

        public OrderResult SendTransaction(Direction direction, int volume, string clientcode)
        {
            TransId++;

            string transactionString = string.Format(CultureInfo.InvariantCulture, "ACCOUNT={0};TYPE=M;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE=0;QUANTITY={5};CLIENT_CODE={6};",
                Account, TransId, ClassCode, SecCode, (char)direction, volume, clientcode);

            double orderNum = 0;

            long replyCode = 0;

            long result = QuikApi.send_sync_transaction_test(transactionString, ref orderNum, ref replyCode);

            return new OrderResult
            {
                OrderNumber = orderNum,
                ReplyCode = (ReplyCode)replyCode,
                ResultCode = (ResultCode)result
            };
        }


        public OrderResult SendTransaction(Direction direction, int volume)
        {
            return SendTransaction(direction, volume, ClientCode);
        }


        public OrderResult SendTransaction(Direction direction, decimal price, int volume)
        {
            return SendTransaction(direction, price, volume, ClientCode);
        }


        public async Task<OrderResult> SendTransactionAsync(Direction direction, decimal price, int volume, string clientcode)
        {
            return await Task<OrderResult>.Factory.StartNew(() =>
                {
                    return SendTransaction(direction, price, volume, clientcode);
                });
        }

        public async Task<OrderResult> SendTransactionAsync(Direction direction, decimal price, int volume)
        {
            return await Task<OrderResult>.Factory.StartNew(() =>
            {
                return SendTransaction(direction, price, volume);
            });
        }


        public OrderResult Buy(decimal price, int volume)
        {
            return SendTransaction(Direction.Buy, price, volume);
        }

        public OrderResult Buy(decimal price, int volume, string clientcode)
        {
            return SendTransaction(Direction.Buy, price, volume, clientcode);
        }

        public OrderResult Buy(int volume, string clientcode)
        {
            return SendTransaction(Direction.Buy, volume, clientcode);
        }

        public OrderResult Buy(int volume)
        {
            return SendTransaction(Direction.Buy, volume);
        }

        public OrderResult Sell(decimal price, int volume)
        {
            return SendTransaction(Direction.Sell, price, volume);
        }

        public OrderResult Sell(decimal price, int volume, string clientcode)
        {
            return SendTransaction(Direction.Sell, price, volume, clientcode);
        }

        public OrderResult Sell(int volume)
        {
            return SendTransaction(Direction.Sell, volume);
        }

        public OrderResult Sell(int volume, string clientcode)
        {
            return SendTransaction(Direction.Sell, volume, clientcode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transId"></param>
        /// <param name="orderNum"></param>
        /// <returns>ReplyCode</returns>
        public OrderResult KillOrder(long transId, double orderNum)
        {
            string kill = string.Format(CultureInfo.InvariantCulture, "CLASSCODE={0}; SECCODE={1}; TRANS_ID={2}; ACTION=KILL_ORDER; ORDER_KEY={3};",
                 ClassCode, SecCode, transId, orderNum);

            long replyCode = 0;

            ResultCode result = (ResultCode)QuikApi.send_sync_transaction_test(kill, ref orderNum, ref replyCode);

            return new OrderResult
            {
                OrderNumber = orderNum,
                ResultCode = result,
                ReplyCode = (ReplyCode)replyCode
            };
        }
    }
}
