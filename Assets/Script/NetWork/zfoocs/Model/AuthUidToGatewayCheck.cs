using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class AuthUidToGatewayCheck
    {
        public long uid;

        public static AuthUidToGatewayCheck ValueOf(long uid)
        {
            var packet = new AuthUidToGatewayCheck();
            packet.uid = uid;
            return packet;
        }
    }


    public class AuthUidToGatewayCheckRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 20;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            AuthUidToGatewayCheck message = (AuthUidToGatewayCheck) packet;
            buffer.WriteInt(-1);
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
            AuthUidToGatewayCheck packet = new AuthUidToGatewayCheck();
            long result0 = buffer.ReadLong();
            packet.uid = result0;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
