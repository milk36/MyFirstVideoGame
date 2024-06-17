using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class GatewaySynchronizeSidAsk
    {
        public string gatewayHostAndPort;
        public Dictionary<long, long> sidMap;

        public static GatewaySynchronizeSidAsk ValueOf(string gatewayHostAndPort, Dictionary<long, long> sidMap)
        {
            var packet = new GatewaySynchronizeSidAsk();
            packet.gatewayHostAndPort = gatewayHostAndPort;
            packet.sidMap = sidMap;
            return packet;
        }
    }


    public class GatewaySynchronizeSidAskRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 24;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            GatewaySynchronizeSidAsk message = (GatewaySynchronizeSidAsk) packet;
            buffer.WriteInt(-1);
            buffer.WriteString(message.gatewayHostAndPort);
            buffer.WriteLongLongMap(message.sidMap);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            GatewaySynchronizeSidAsk packet = new GatewaySynchronizeSidAsk();
            string result0 = buffer.ReadString();
            packet.gatewayHostAndPort = result0;
            var map1 = buffer.ReadLongLongMap();
            packet.sidMap = map1;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
