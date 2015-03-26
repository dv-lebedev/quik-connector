using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDde.Server;

namespace QuikConnector.Data
{
    public class QDataServer : DdeServer
    {
         public Dictionary<string, DataChannel> Channels { get; protected set; }

         public QDataServer(string service)
             : base(service)
        {
            Channels = new Dictionary<string, DataChannel>(); 
        }

        public void AddChannel(string topic, DataChannel channel)
        {
            Channels.Add(topic, channel);
        }

        protected override bool OnBeforeConnect(string topic)
        {
            return Channels.ContainsKey(topic);
        }

        protected override void OnAfterConnect(DdeConversation c)
        {
            DataChannel channel = Channels[c.Topic];
            c.Tag = channel;
            channel.IsConnected = true;
        }

        protected override void OnDisconnect(DdeConversation c)
        {
            ((DataChannel)c.Tag).IsConnected = false;
        }

        protected override PokeResult OnPoke(DdeConversation c, string item, byte[] data, int format)
        {
            ((DataChannel)c.Tag).PutDdeData(data);
            return PokeResult.Processed;
        }
    }
}
