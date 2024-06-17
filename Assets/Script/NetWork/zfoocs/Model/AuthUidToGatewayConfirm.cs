using System;
using System.Collections.Generic;

namespace zfoocs
{
    
    public class AuthUidToGatewayConfirm
    {
        public long uid;

        public static AuthUidToGatewayConfirm ValueOf(long uid)
        {
            var packet = new AuthUidToGatewayConfirm();
            packet.uid = uid;
            return packet;
        }
    }


    public class AuthUidToGatewayConfirmRegistration : IProtocolRegistration
    {
        public short ProtocolId()
        {
            return 21;
        }

        public void Write(ByteBuffer buffer, object packet)
        {
            if (packet == null)
            {
                buffer.WriteInt(0);
                return;
            }
            AuthUidToGatewayConfirm message = (AuthUidToGatewayConfirm) packet;
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
            AuthUidToGatewayConfirm packet = new AuthUidToGatewayConfirm();
            long result0 = buffer.ReadLong();
            packet.uid = result0;
            if (length > 0) {
                buffer.SetReadOffset(beforeReadIndex + length);
            }
            return packet;
        }
    }
}
