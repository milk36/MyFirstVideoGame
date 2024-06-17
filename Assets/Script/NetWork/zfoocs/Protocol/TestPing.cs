using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class TestPing
    {
        

        public static TestPing ValueOf()
        {
            var packet = new TestPing();
            
            return packet;
        }
    }


    public class TestPingRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 1000;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            TestPing message = (TestPing) packet;
            buffer.WriteInt(-1);
        }

        public object Read(ByteBuffer buffer)
        {
            int length = buffer.ReadInt();
            if (length == 0)
            {
                return null;
            }
            int beforeReadIndex = buffer.ReadOffset();
            TestPing packet = new TestPing();
            
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
