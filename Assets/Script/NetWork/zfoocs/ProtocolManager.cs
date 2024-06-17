using System;
using System.Collections.Generic;

namespace zfoocs
{
    public class ProtocolManager
    {
        public static readonly short MAX_PROTOCOL_NUM = short.MaxValue;


        private static readonly IProtocolRegistration[] protocols = new IProtocolRegistration[MAX_PROTOCOL_NUM];
        private static readonly Dictionary<Type, short> protocolIdMap = new Dictionary<Type, short>();


        public static void InitProtocol()
        {
            protocols[0] = new SignalAttachmentRegistration();
            protocolIdMap[typeof(SignalAttachment)] = 0;
            protocols[2] = new GatewayAttachmentRegistration();
            protocolIdMap[typeof(GatewayAttachment)] = 2;
            protocols[3] = new UdpAttachmentRegistration();
            protocolIdMap[typeof(UdpAttachment)] = 3;
            protocols[4] = new HttpAttachmentRegistration();
            protocolIdMap[typeof(HttpAttachment)] = 4;
            protocols[5] = new NoAnswerAttachmentRegistration();
            protocolIdMap[typeof(NoAnswerAttachment)] = 5;
            protocols[20] = new AuthUidToGatewayCheckRegistration();
            protocolIdMap[typeof(AuthUidToGatewayCheck)] = 20;
            protocols[21] = new AuthUidToGatewayConfirmRegistration();
            protocolIdMap[typeof(AuthUidToGatewayConfirm)] = 21;
            protocols[22] = new AuthUidAskRegistration();
            protocolIdMap[typeof(AuthUidAsk)] = 22;
            protocols[23] = new GatewaySessionInactiveAskRegistration();
            protocolIdMap[typeof(GatewaySessionInactiveAsk)] = 23;
            protocols[24] = new GatewaySynchronizeSidAskRegistration();
            protocolIdMap[typeof(GatewaySynchronizeSidAsk)] = 24;
            protocols[100] = new MessageRegistration();
            protocolIdMap[typeof(Message)] = 100;
            protocols[101] = new ErrorRegistration();
            protocolIdMap[typeof(Error)] = 101;
            protocols[102] = new HeartbeatRegistration();
            protocolIdMap[typeof(Heartbeat)] = 102;
            protocols[103] = new PingRegistration();
            protocolIdMap[typeof(Ping)] = 103;
            protocols[104] = new PongRegistration();
            protocolIdMap[typeof(Pong)] = 104;
            protocols[110] = new PairIntLongRegistration();
            protocolIdMap[typeof(PairIntLong)] = 110;
            protocols[111] = new PairLongRegistration();
            protocolIdMap[typeof(PairLong)] = 111;
            protocols[112] = new PairStringRegistration();
            protocolIdMap[typeof(PairString)] = 112;
            protocols[113] = new PairLSRegistration();
            protocolIdMap[typeof(PairLS)] = 113;
            protocols[114] = new TripleLongRegistration();
            protocolIdMap[typeof(TripleLong)] = 114;
            protocols[115] = new TripleStringRegistration();
            protocolIdMap[typeof(TripleString)] = 115;
            protocols[116] = new TripleLSSRegistration();
            protocolIdMap[typeof(TripleLSS)] = 116;
            protocols[1000] = new TestPingRegistration();
            protocolIdMap[typeof(TestPing)] = 1000;
            protocols[1001] = new TestPongRegistration();
            protocolIdMap[typeof(TestPong)] = 1001;
        }

        public static IProtocolRegistration GetProtocol(short protocolId)
        {
            var protocol = protocols[protocolId];
            if (protocol == null)
            {
                throw new Exception("[protocolId:" + protocolId + "] not exist");
            }

            return protocol;
        }

        public static void Write(ByteBuffer buffer, object packet)
        {
            var protocolId = protocolIdMap[packet.GetType()];
            // 写入协议号
            buffer.WriteShort(protocolId);

            // 写入包体
            GetProtocol(protocolId).Write(buffer, packet);
        }

        public static object Read(ByteBuffer buffer)
        {
            var protocolId = buffer.ReadShort();
            return GetProtocol(protocolId).Read(buffer);
        }
    }
}