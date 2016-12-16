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
using System.Globalization;
using System.Threading.Tasks;
using QuikConnector.Orders;

namespace QuikConnector
{
    [Serializable]
    public class OrderChannel 
    {
        public static long TransId { get; private set; }

        public string Account { get; private set; }
        public string SecCode { get; private set; }
        public string ClassCode { get; private set; }

        public SortedDictionary<double, OrderCallbackEventArgs> Orders { get; protected set; }
        public SortedDictionary<double, TradeCallbackEventArgs> Trades { get; protected set; }

        public event EventHandler<OrderCallbackEventArgs> OrderCallback;
        public event EventHandler<TradeCallbackEventArgs> TradeCallback;

        protected OrderChannel()
        {
            Orders = new SortedDictionary<double, OrderCallbackEventArgs>();
            Trades = new SortedDictionary<double, TradeCallbackEventArgs>();
        }

        public OrderChannel(string account, string secCode, string classCode)
            : this()
        {
            if (account == null) throw new ArgumentNullException("account");
            if (secCode == null) throw new ArgumentNullException("secCode");
            if (classCode == null) throw new ArgumentNullException("classCode");

            Account = account;
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

            OrderCallback?.Invoke(this, e);
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

            TradeCallback?.Invoke(this, e);
        }

        public OrderResult SendTransaction(Direction direction, decimal price, int volume, string clientcode = "")
        {
            TransId++;

            string transactionString = string.Format(CultureInfo.InvariantCulture, "ACCOUNT={0};TYPE=L;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE={5};QUANTITY={6};CLIENT_CODE={7};",
                Account, TransId, ClassCode, SecCode, (char)direction, price, volume, clientcode);

            double orderNum = 0;

            long replyCode = 0;

            long result = QuikApi.send_sync_transaction_test(transactionString, ref orderNum, ref replyCode);

            return new OrderResult(TransId, orderNum, (ReplyCode)replyCode, (ResultCode)result);
        }

        public OrderResult SendTransaction(Direction direction, int volume, string clientcode = "")
        {
            TransId++;

            string transactionString = string.Format(CultureInfo.InvariantCulture, "ACCOUNT={0};TYPE=M;TRANS_ID={1};CLASSCODE={2};SECCODE={3};ACTION=NEW_ORDER;OPERATION={4};PRICE=0;QUANTITY={5};CLIENT_CODE={6};",
                Account, TransId, ClassCode, SecCode, (char)direction, volume, clientcode);

            double orderNum = 0;

            long replyCode = 0;

            long result = QuikApi.send_sync_transaction_test(transactionString, ref orderNum, ref replyCode);

            return new OrderResult(TransId, orderNum, (ReplyCode)replyCode, (ResultCode)result);
        }

        public Task<OrderResult> SendTransactionAsync(Direction direction, decimal price, int volume, string clientcode = "")
        {
            return Task<OrderResult>.Factory.StartNew(() =>
                {
                    return SendTransaction(direction, price, volume, clientcode);
                });
        }

        public OrderResult Buy(decimal price, int volume, string clientcode = "") => SendTransaction(Direction.Buy, price, volume, clientcode);
        public OrderResult Buy(int volume, string clientcode = "") => SendTransaction(Direction.Buy, volume, clientcode);
        
        public OrderResult Sell(decimal price, int volume, string clientcode = "") => SendTransaction(Direction.Sell, price, volume, clientcode);
        public OrderResult Sell(int volume, string clientcode = "") => SendTransaction(Direction.Sell, volume, clientcode);

        public OrderResult KillOrder(OrderResult orderResult)
        {
            return KillOrder(orderResult.TransId, orderResult.OrderNumber);
        }

        public OrderResult KillOrder(long transId, double orderNum)
        {
            if (!Orders.ContainsKey(orderNum))
                throw new ArgumentException(string.Format("Order for {0} with orderNum = {1} is NOT FOUND."));

            string kill = string.Format(CultureInfo.InvariantCulture, "CLASSCODE={0}; SECCODE={1}; TRANS_ID={2}; ACTION=KILL_ORDER; ORDER_KEY={3};",
                 ClassCode, SecCode, transId, orderNum);

            long replyCode = 0;

            ResultCode result = (ResultCode)QuikApi.send_sync_transaction_test(kill, ref orderNum, ref replyCode);

            return new OrderResult(transId, orderNum, (ReplyCode)replyCode, result);
        }
    }
}
