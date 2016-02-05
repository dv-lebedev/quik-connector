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
using System.Linq;

namespace QuikConnector
{
    [Serializable]
    public class QuikConnection : IDisposable
    {
        public string PathToQuik { get; set; }

        public List<OrderChannel> Channels { get; protected set; }

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler Disposed;

        public string Account { get; set; }

        public bool IsQuikConnected => QuikApi.IsQuikConnected(); 

        public bool IsDllConnected => QuikApi.IsDLLConnected();

        public bool IsConnected => QuikApi.IsQuikConnected() && QuikApi.IsDLLConnected();
 
        protected QuikConnection()
        {
            Channels = new List<OrderChannel>();

            QuikApi.OrderCallback += OnOrderCallback;
            QuikApi.TradeCallBack += OnTradeCallback;
            QuikApi.ConnectionStatusCallback += OnStatusCallback;
        }

        public QuikConnection(string path, string account)
            : this()
        {
            PathToQuik = new Uri(path).LocalPath;
            Account = account;
        }


        public bool Connect()
        {
            if (QuikApi.TRANS2QUIK_SUCCESS
                == QuikApi.Connect(PathToQuik))
            {
                OnConnected(this, null);
                QuikApi.MultiSubscribe();

                return true;
            }

            return false;
        }

        public bool Disconnect()
        {
            if (QuikApi.TRANS2QUIK_SUCCESS
                == QuikApi.Disconnect())
            {
                OnDisconnected(this, null);
                return true;
            }

            return false;
        }


        public OrderChannel CreateOrderChannel(string secCode, string classCode)
        {
            var channel = new OrderChannel(Account, secCode, classCode);

            Subscribe(channel);

            return channel;
        }


        public void Subscribe(OrderChannel channel)
        {
            Channels.Add(channel);
        }


        public bool Unsubscribe(OrderChannel channel)
        {
            return Channels.Remove(channel);
        }


        public int Unsubscribe(Predicate<OrderChannel> channels)
        {
            return Channels.RemoveAll(channels);
        }


        public void UnsubscribeAllOrders()
        {
            Channels.Clear();
        }


        public void Dispose()
        {
            if (IsConnected) Disconnect();

            QuikApi.OrderCallback -= OnOrderCallback;
            QuikApi.TradeCallBack -= OnTradeCallback;
            QuikApi.ConnectionStatusCallback -= OnStatusCallback;

            OnDisposed(this, null);
        }


        protected void OnOrderCallback(object sender, OrderCallbackEventArgs value)
        {
            foreach (var item in Channels.Where(i => i.SecCode == value.SecCode))
            {
                item.OnOrderCallback(value);
            }
        }

        protected void OnTradeCallback(object sender, TradeCallbackEventArgs value)
        {
            foreach (var item in Channels.Where(i => i.SecCode == value.SecCode))
            {
                item.OnTradeCallback(value);
            }
        }

        protected void OnStatusCallback(object sender, ConnectionStatusEventArgs cscp)
        {
            ConnectionStatusChanged?.Invoke(sender, cscp);
        }

        protected void OnConnected(object sender, EventArgs e)
        {
            Connected?.Invoke(sender, e);
        }

        protected void OnDisconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(sender, e);
        }

        protected void OnDisposed(object sender, EventArgs e)
        {
            Disposed?.Invoke(sender, e);
        }
    }
}
