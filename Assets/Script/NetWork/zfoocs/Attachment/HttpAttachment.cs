using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class HttpAttachment
    {
        public long uid;
        public int taskExecutorHash;

        public static HttpAttachment ValueOf(int taskExecutorHash, long uid)
        {
            var packet = new HttpAttachment();
            packet.taskExecutorHash = taskExecutorHash;
            packet.uid = uid;
            return packet;
        }
    }


    public class HttpAttachmentRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 4;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            HttpAttachment message = (HttpAttachment) packet;
            buffer.WriteInt(-1);
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
            HttpAttachment packet = new HttpAttachment();
            int result0 = buffer.ReadInt();
            packet.taskExecutorHash = result0;
            long result1 = buffer.ReadLong();
            packet.uid = result1;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
