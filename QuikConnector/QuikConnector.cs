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
using System.Reflection;
using QuikConnector.API;
using QuikConnector.Data;
using QuikConnector.Exceptions;

namespace QuikConnector.Core
{
    public class QConnector : IDisposable
    {
        public string Account
        {
            get
            {
                return connection.Account;
            }

            set
            {
                connection.Account = value;
            }
        }

        public string ClientCode
        {
            get
            {
                return connection.ClientCode;
            }

            set
            {
                connection.ClientCode = value;
            }
        }

        private QuikConnection connection;

        private QDataServer server;

        #region EVENTS

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<ConnectionStatusEventArgs> ConnectionStatusChanged;

        public event EventHandler ImportStarted;
        public event EventHandler ImportStopped;

        public event EventHandler<OrderChannel> OrderChannelAdded;
        public event EventHandler<OrderChannel> OrderChannelRemoved;
        public event EventHandler<OrderChannel> OrderChannelCreated;

        public event EventHandler<DataChannel> DataChannelAdded;
        public event EventHandler DataChannelRemoved;

        public event EventHandler Disposed;


        protected virtual void OnConnected(object sender, EventArgs e)
        {
            Connected?.Invoke(sender, e);
        }

        protected virtual void OnDisconnected(object sender, EventArgs e)
        {
            Disconnected?.Invoke(sender, e);
        }

        protected virtual void OnImportStarted(object sender, EventArgs e)
        {
            ImportStarted?.Invoke(sender, e);
        }

        protected virtual void OnImportStopped(object sender, EventArgs e)
        {
            ImportStopped?.Invoke(sender, e);
        }

        protected virtual void OnOrderChannelAdded(object sender, OrderChannel e)
        {
            OrderChannelAdded?.Invoke(sender, e);
        }

        protected virtual void OnOrderChannelRemoved(object sender, OrderChannel e)
        {
            OrderChannelRemoved?.Invoke(sender, e);
        }

        protected virtual void OnOrderChannelCreated(object sender, OrderChannel e)
        {
            OrderChannelCreated?.Invoke(sender, e);
        }

        protected virtual void OnDataChannelAdded(object sender, DataChannel e)
        {
            DataChannelAdded?.Invoke(sender, e);
        }

        protected virtual void OnDataChannelRemoved(object sender, EventArgs e)
        {
            DataChannelRemoved?.Invoke(sender, e);
        }

        protected virtual void OnConnectionStatusChanged(object sender, ConnectionStatusEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(sender, e);
        }

        protected virtual void OnDisposed(object sender, EventArgs e)
        {
            Disposed?.Invoke(sender, e);
        }


        #endregion

        public List<OrderChannel> OrderChannels
        {
            get
            {
                return connection.Channels;
            }
        }

        public Dictionary<string, DataChannel> DataChannels
        {
            get
            {
                return server.Channels;
            }
        }
 
        public QConnector(ConnectorParameters parameters)
        {
            connection = new QuikConnection(parameters.Path)
            {
                Account = parameters.Account,
                ClientCode = parameters.ClientCode
            };


            server = new QDataServer(parameters.ServerName);

        }

        public bool Connect()
        {
            if(connection.Connect())
            {
                OnConnected(this, null);

                return true;
            }

            return false;
        }

        public bool Disconnect()
        {
            if (connection.Disconnect())
            {
                OnDisconnected(this, null);

                return true;
            }

            return false;
        }

        public void StartImport()
        {
            server.Register();

            Terminal.StartDDE();

            OnImportStarted(this, null);
        }

        public void StopImport()
        {
            server.Unregister();

            Terminal.StopDDE();

            OnImportStopped(this, null);
        }

        public void AddDataChannel(string key, DataChannel value)
        {
            server.Channels.Add(key, value);

            OnDataChannelAdded(this, value);
        }

        public DataTable<T> AddDataTable<T>() where T : new()
        {
            QuikTableAttribute attr = typeof(T).GetTypeInfo().GetCustomAttribute<QuikTableAttribute>();

            if (attr == null)
                throw new AttributeNotFoundException(typeof(QuikTableAttribute));

            DataTable<T> table = new DataTable<T>();

            AddDataChannel(attr.Name, table);

            return table;
        }

        public bool RemoveDataChannel(string key)
        {
            if (server.Channels.Remove(key))
            {
                OnDataChannelRemoved(this, null);

                return true;
            }

            return false;
        }

        public bool RemoveDataTable<T>()
        {
            QuikTableAttribute attr = typeof(T).GetTypeInfo().GetCustomAttribute<QuikTableAttribute>();

            if (attr == null)
                throw new AttributeNotFoundException(typeof(QuikTableAttribute));

            return RemoveDataChannel(attr.Name);
        }

        public void AddOrderChannel(OrderChannel channel)
        {
            connection.Subscribe(channel);

            OnOrderChannelAdded(this, channel);
        }

        public void RemoveOrderChannel(string secCode)
        {
            connection.Unsubscribe(i => i.SecCode == secCode);
          
            OnOrderChannelRemoved(this, null);
        }

        public bool RemoveOrderChannel(OrderChannel channel)
        {
            if (connection.Channels.Remove(channel))
            {
                OnOrderChannelRemoved(this, channel);

                return true;
            }

            return false;
        }

        public OrderChannel CreateOrderChannel(string secCode, string classCode)
        {
            var channel = connection.CreateOrderChannel(secCode, classCode);

            if(channel != null)
            {
                OrderChannelCreated(this, channel);
            }

            return channel;
        }

        public void Dispose()
        {
            if (connection != null) connection.Dispose();

            if (server != null) server.Dispose();

            OnDisposed(this, null);
        }
    }
}

