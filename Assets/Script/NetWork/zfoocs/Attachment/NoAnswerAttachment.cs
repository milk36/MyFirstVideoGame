using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class NoAnswerAttachment
    {
        public int taskExecutorHash;

        public static NoAnswerAttachment ValueOf(int taskExecutorHash)
        {
            var packet = new NoAnswerAttachment();
            packet.taskExecutorHash = taskExecutorHash;
            return packet;
        }
    }


    public class NoAnswerAttachmentRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 5;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            NoAnswerAttachment message = (NoAnswerAttachment) packet;
            buffer.WriteInt(-1);
            buffer.WriteInt(message.taskExecutorHash);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            NoAnswerAttachment packet = new NoAnswerAttachment();
            int result0 = buffer.ReadInt();
            packet.taskExecutorHash = result0;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
