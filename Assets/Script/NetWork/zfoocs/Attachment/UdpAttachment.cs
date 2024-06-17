using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class UdpAttachment
    {
        public string host;
        public int port;

        public static UdpAttachment ValueOf(string host, int port)
        {
            var packet = new UdpAttachment();
            packet.host = host;
            packet.port = port;
            return packet;
        }
    }


    public class UdpAttachmentRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 3;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            UdpAttachment message = (UdpAttachment) packet;
            buffer.WriteInt(-1);
            buffer.WriteString(message.host);
            buffer.WriteInt(message.port);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            UdpAttachment packet = new UdpAttachment();
            string result0 = buffer.ReadString();
            packet.host = result0;
            int result1 = buffer.ReadInt();
            packet.port = result1;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
