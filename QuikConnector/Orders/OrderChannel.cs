using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QuikConnector
{
    public class OrderChannel
    {
        public static long TransId { get; private set; }

        public AccountParameters AccountParameters { get; protected set; }

        public string SecCode { get; set; }
        public string ClassCode { get; set; }

        public Dictionary<double, OrderCallbackEventArgs> OrderCallbacks { get; protected set; }

        public OrderCallbackEventArgs LastOrderCallback { get; private set; }

        public event EventHandler<OrderCallbackEventArgs> OrderCallback;

        public event EventHandler<TradeCallbackEventArgs> TradeCallback;


        protected OrderChannel()
        {
            LastOrderCallback = new OrderCallbackEventArgs();
            OrderCallbacks = new Dictionary<double, OrderCallbackEventArgs>();
        }


        public OrderChannel(AccountParameters account, string secCode, string classCode)
            : this()
        {
            AccountParameters = account;
            SecCode = secCode;
            ClassCode = classCode;
        }

        public OrderChannel(string account, string clientCode,
            string secCode, string classCode)
            : this()
        {
            AccountParameters.Account = account;
            AccountParameters.ClientCode = clientCode;
            SecCode = secCode;
            ClassCode = classCode;
        }


        public void OnOrderCallback(OrderCallbackEventArgs e)
        {
            LastOrderCallback = e;

            OrderCallbackEventArgs value;

            if (OrderCallbacks.TryGetValue(e.Number, out value))
            {
                value = e;
            }
            else
            {
                OrderCallbacks.Add(e.Number, e);
            }

            if (OrderCallback != null) OrderCallback(this, e);
        }

        public void OnTradeCallback(TradeCallbackEventArgs e)
        {
            if (TradeCallback != null) TradeCallback(this, e);
        }


        public double SendTransaction(Direction direction, decimal price, int volume)
        {
            TransId++;

            string transactionString = string.Format("ACCOUNT={0};TYPE=L;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE={5};QUANTITY={6};CLIENT_CODE={7};",
                AccountParameters.Account, TransId, ClassCode, SecCode, (char)direction, price, volume, AccountParameters.ClientCode);

            double orderNum = 0;

            QuikApi.send_sync_transaction_test(transactionString, ref orderNum);

            return orderNum;
        }

        public bool SendTransaction(Direction direction, decimal price, int volume, int checkIterationCount, int checkIterationDelay)
        {
            TransId++;

            string transactionString = string.Format("ACCOUNT={0};TYPE=L;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE={5};QUANTITY={6};CLIENT_CODE={7};",
                AccountParameters.Account, TransId, ClassCode, SecCode, (char)direction, price, volume, AccountParameters.ClientCode);

            double orderNum = 0;

            QuikApi.send_sync_transaction_test(transactionString, ref orderNum);

            int count = 0;

            OrderCallbackEventArgs value;

            while (count++ != checkIterationCount)
            {
                if (OrderCallbacks.TryGetValue(orderNum, out value) && value.Status == 0)
                {
                    return true;
                }

                Thread.Sleep(checkIterationDelay);
            }

            string kill = string.Format("CLASSCODE={0}; SECCODE={1}; TRANS_ID={2}; ACTION=KILL_ORDER; ORDER_KEY={3};",
                ClassCode, SecCode, TransId, orderNum);

            QuikApi.send_sync_transaction_test(kill, ref orderNum);


            return false;
        }

        public Task<double> SendTransactionAsync(Direction direction, decimal price, int volume)
        {
            return Task<double>.Factory.StartNew(() =>
                {
                    return SendTransaction(direction, price, volume);
                });
        }

        public Task<bool> SendTransactionAsync(Direction direction, decimal price, int volume, int checkIterationCount, int checkIterationDelay)
        {
            return Task<bool>.Factory.StartNew(() =>
                {
                    return SendTransaction(direction, price, volume, checkIterationCount, checkIterationDelay);
                });
        }


        public double Buy(decimal price, int volume)
        {
            return SendTransaction(Direction.Buy, price, volume);
        }

        public double Sell(decimal price, int volume)
        {
            return SendTransaction(Direction.Sell, price, volume);
        }


        public double KillOrder(long transId, double orderNum)
        {
            string kill = string.Format("CLASSCODE={0}; SECCODE={1}; TRANS_ID={2}; ACTION=KILL_ORDER; ORDER_KEY={3};",
                 ClassCode, SecCode, transId, orderNum);

            QuikApi.send_sync_transaction_test(kill, ref orderNum);

            return orderNum;
        }
    }
}
