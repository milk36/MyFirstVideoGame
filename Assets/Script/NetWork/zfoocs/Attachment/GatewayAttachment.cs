using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class GatewayAttachment
    {
        public long sid;
        public long uid;
        public int taskExecutorHash;
        public bool client;
        public SignalAttachment signalAttachment;

        public static GatewayAttachment ValueOf(bool client, long sid, SignalAttachment signalAttachment, int taskExecutorHash, long uid)
        {
            var packet = new GatewayAttachment();
            packet.client = client;
            packet.sid = sid;
            packet.signalAttachment = signalAttachment;
            packet.taskExecutorHash = taskExecutorHash;
            packet.uid = uid;
            return packet;
        }
    }


    public class GatewayAttachmentRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 2;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            GatewayAttachment message = (GatewayAttachment) packet;
            buffer.WriteInt(-1);
            buffer.WriteBool(message.client);
            buffer.WriteLong(message.sid);
            buffer.WritePacket(message.signalAttachment, 0);
            buffer.WriteInt(message.taskExecutorHash);
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
            GatewayAttachment packet = new GatewayAttachment();
            bool result0 = buffer.ReadBool();
            packet.client = result0;
            long result1 = buffer.ReadLong();
            packet.sid = result1;
            SignalAttachment result2 = buffer.ReadPacket<SignalAttachment>(0);
            packet.signalAttachment = result2;
            int result3 = buffer.ReadInt();
            packet.taskExecutorHash = result3;
            long result4 = buffer.ReadLong();
            packet.uid = result4;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
