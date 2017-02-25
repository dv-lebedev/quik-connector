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
        private QuikConnection _connection;
        private QDataServer _server;

        public List<OrderChannel> OrderChannels => _connection.Channels;
        public Dictionary<string, DataChannel> DataChannels => _server.Channels;
        public string Account => _connection.Account;
        public QuikConnection Connection => _connection;

        #region EVENTS

        public event EventHandler ImportStarted;
        public event EventHandler ImportStopped;

        public event EventHandler<OrderChannel> OrderChannelAdded;
        public event EventHandler<OrderChannel> OrderChannelRemoved;
        public event EventHandler<OrderChannel> OrderChannelCreated;

        public event EventHandler<DataChannel> DataChannelAdded;
        public event EventHandler DataChannelRemoved;

        public event EventHandler Disposed;

        protected virtual void OnImportStarted(object sender, EventArgs e) => ImportStarted?.Invoke(sender, e);
        protected virtual void OnImportStopped(object sender, EventArgs e) => ImportStopped?.Invoke(sender, e);
        protected virtual void OnOrderChannelAdded(object sender, OrderChannel e) => OrderChannelAdded?.Invoke(sender, e);
        protected virtual void OnOrderChannelRemoved(object sender, OrderChannel e) => OrderChannelRemoved?.Invoke(sender, e);
        protected virtual void OnOrderChannelCreated(object sender, OrderChannel e) => OrderChannelCreated?.Invoke(sender, e);
        protected virtual void OnDataChannelAdded(object sender, DataChannel e) => DataChannelAdded?.Invoke(sender, e);
        protected virtual void OnDataChannelRemoved(object sender, EventArgs e)  => DataChannelRemoved?.Invoke(sender, e);
        protected virtual void OnDisposed(object sender, EventArgs e) => Disposed?.Invoke(sender, e);


        #endregion

        public QConnector(ConnectorParameters parameters)
        {
            _connection = new QuikConnection(parameters.PathToQuik, parameters.Account);
            _server = new QDataServer(parameters.ServerName);
        }

        public void Connect() => _connection.Connect();
        public void Disconnect() => _connection.Disconnect();

        public void StartImport()
        {
            _server.Register();
            OnImportStarted(this, null);
        }

        public void StopImport()
        {
            _server.Unregister();
            OnImportStopped(this, null);
        }

        public void AddDataChannel(string key, DataChannel value)
        {
            _server.Channels.Add(key, value);
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
            if (_server.Channels.Remove(key))
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
            _connection.Subscribe(channel);

            OnOrderChannelAdded(this, channel);
        }

        public void RemoveOrderChannel(string secCode)
        {
            _connection.Unsubscribe(i => i.SecCode == secCode);

            OnOrderChannelRemoved(this, null);
        }

        public bool RemoveOrderChannel(OrderChannel channel)
        {
            if (_connection.Channels.Remove(channel))
            {
                OnOrderChannelRemoved(this, channel);

                return true;
            }

            return false;
        }

        public OrderChannel CreateOrderChannel(string secCode, string classCode)
        {
            var channel = _connection.CreateOrderChannel(secCode, classCode);

            if (channel != null)
            {
                OnOrderChannelCreated(this, channel);
            }

            return channel;
        }

        public void Dispose()
        {
            if (_connection != null) _connection.Dispose();

            if (_server != null) _server.Dispose();

            OnDisposed(this, null);
        }
    }
}

