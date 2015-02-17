using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuikConnector
{
    public class QuikConnection : IDisposable
    {
        public string PathToQuik { get; set; }

        public List<OrderChannel> Orders { get; protected set; }

        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public event EventHandler Connected;

        public event EventHandler Disconnected;

        public event EventHandler Disposed;

        public bool IsQuikConnected
        {
            get 
            { 
                return QuikApi.IsQuikConnected(); 
            }
        }

        public bool IsDllConnected
        {
            get 
            {
                return QuikApi.IsDLLConnected();
            }
        }

        public bool IsConnected
        {
            get
            {
                return QuikApi.IsQuikConnected() && QuikApi.IsDLLConnected();
            }
        }

        protected QuikConnection()
        {
            Orders = new List<OrderChannel>();

            QuikApi.OrderCallback += OnOrderCallback;
            QuikApi.TradeCallBack += OnTradeCallback;
            QuikApi.ConnectionStatusCallback += OnStatusCallback;
        }

        public QuikConnection(string path)
            : this()
        {
            PathToQuik = new Uri(path).LocalPath;
        }


        public QuikConnection(bool tryToFindPath = true)
            : this()
        {
            if (tryToFindPath)
            {
                if ((PathToQuik = GetPathToActiveQuik()) == null)
                {
                    throw new ExecutingTerminalNotFound();
                }
            }
        }

        public string GetPathToActiveQuik()
        {
            return Process.GetProcessesByName("info")
                .FirstOrDefault()
                .MainModule
                .FileName
                .Replace("info.exe", "");
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

        public void Subscribe(OrderChannel order)
        {
            Orders.Add(order);
        }

        public void Unsubscribe(OrderChannel order)
        {
            Orders.Remove(order);
        }

        public void UnsubscribeAllOrders()
        {
            Orders.Clear();
        }

        public void UnsubscribeAllOrders(Predicate<OrderChannel> match)
        {
            Orders.RemoveAll(match);
        }


        public void Dispose()
        {
            if (IsConnected) this.Disconnect();

            QuikApi.OrderCallback -= OnOrderCallback;
            QuikApi.TradeCallBack -= OnTradeCallback;
            QuikApi.ConnectionStatusCallback -= OnStatusCallback;

            OnDisposed(this, null);
        }


        protected void OnOrderCallback(object sender, OrderCallbackEventArgs value)
        {
            foreach (OrderChannel order in Orders.Where(x => x.SecCode == value.SecCode))
            {
                order.OnOrderCallback(value);
            }
        }

        protected void OnTradeCallback(object sender, TradeCallbackEventArgs value)
        {
            foreach (OrderChannel order in Orders.Where(x => x.SecCode == value.SecCode))
            {
                order.OnTradeCallback(value);
            }
        }

        protected void OnStatusCallback(object sender, ConnectionStatusEventArgs cscp)
        {
            if (ConnectionStatusChanged != null) ConnectionStatusChanged(sender, cscp);
        }

        protected void OnConnected(object sender, EventArgs e)
        {
            if (Connected != null) Connected(sender, e);
        }

        protected void OnDisconnected(object sender, EventArgs e)
        {
            if (Disconnected != null) Disconnected(sender, e);
        }


        protected void OnDisposed(object sender, EventArgs e)
        {
            if (Disposed != null) Disposed(sender, e);
        }
    }
}
