using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QuikConnector
{
    public class QuikConnection : IDisposable
    {
        public string PathToQuik { get; set; }

        public ICollection<OrderChannel> Channels { get; protected set; }

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
            Channels = new List<OrderChannel>();

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

        public void Subscribe(OrderChannel channel)
        {
            Channels.Add(channel);
        }


        public bool Unsubscribe(OrderChannel channel)
        {
            return Channels.Remove(channel);
        }

        public void UnsubscribeAllOrders()
        {
            Channels.Clear();
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
