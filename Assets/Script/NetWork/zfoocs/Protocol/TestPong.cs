using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class TestPong
    {
        public long time;

        public static TestPong ValueOf(long time)
        {
            var packet = new TestPong();
            packet.time = time;
            return packet;
        }
    }


    public class TestPongRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1001;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            TestPong message = (TestPong) packet;
            buffer.WriteInt(-1);
            buffer.WriteLong(message.time);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            TestPong packet = new TestPong();
            long result0 = buffer.ReadLong();
            packet.time = result0;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
