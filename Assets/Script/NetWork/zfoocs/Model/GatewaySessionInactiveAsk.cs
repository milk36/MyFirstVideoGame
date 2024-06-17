using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class GatewaySessionInactiveAsk
    {
        public string gatewayHostAndPort;
        public long sid;
        public long uid;

        public static GatewaySessionInactiveAsk ValueOf(string gatewayHostAndPort, long sid, long uid)
        {
            var packet = new GatewaySessionInactiveAsk();
            packet.gatewayHostAndPort = gatewayHostAndPort;
            packet.sid = sid;
            packet.uid = uid;
            return packet;
        }
    }


    public class GatewaySessionInactiveAskRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 23;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            GatewaySessionInactiveAsk message = (GatewaySessionInactiveAsk) packet;
            buffer.WriteInt(-1);
            buffer.WriteString(message.gatewayHostAndPort);
            buffer.WriteLong(message.sid);
            buffer.WriteLong(message.uid);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            GatewaySessionInactiveAsk packet = new GatewaySessionInactiveAsk();
            string result0 = buffer.ReadString();
            packet.gatewayHostAndPort = result0;
            long result1 = buffer.ReadLong();
            packet.sid = result1;
            long result2 = buffer.ReadLong();
            packet.uid = result2;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
