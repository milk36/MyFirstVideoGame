using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using zfoocs;

namespace NetWork
{
  public class NetTcpClient
  {
    private const int PROTOCOL_HEAD_LENGTH = 4;
    private string _hostAddress;
    private int _port;
    private NetPlayer _netPlayer;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _readThread;
    private Thread _sendThread;
    private bool _connected2Server = false;

    private int _maxMessageSize = 16 * 1024;
    public ConcurrentQueue<Message> _receiveQueue = new();
    private ConcurrentQueue<byte[]> _sendQueue = new();

    // public bool IsConnected2Server
    // {
    //   get { return _connected2Server; }
    // }

    public NetTcpClient(NetPlayer netPlayer, string hostAddress, int port)
    {
      ProtocolManager.InitProtocol();
      _netPlayer = netPlayer;
      _hostAddress = hostAddress;
      _port = port;
    }

    public void ConnectToServer()
    {
      if (_connected2Server) return;


      _client = new TcpClient(_hostAddress, _port);
      _stream = _client.GetStream();

      Debug.Log("Connected to server.");

      _connected2Server = true;

      //接收数据线程
      _readThread = new Thread(new ThreadStart(ReadLoop));
      _readThread.IsBackground = true;
      _readThread.Start();

      // 发送数据线程
      _sendThread = new Thread(SendLoop);
      _sendThread.IsBackground = true;
      _sendThread.Start();
    }

    private void ReadLoop()
    {
      try
      {
        while (true)
        {
          byte[] content;
          if (!ReadMessageBlocking(out content))
          {
            // break instead of return so stream close still happens!
            break;
          }

          ByteBuffer byteBuffer = null;
          try
          {
            byteBuffer = ByteBuffer.ValueOf();
            byteBuffer.WriteBytes(content);
            object msg = ProtocolManager.Read(byteBuffer);
            // 将读取的协议数据添加到队列中 
            _receiveQueue.Enqueue(new Message(MessageType.Data, msg));
          }
          finally
          {
            if (byteBuffer != null)
            {
              byteBuffer.Clear();
            }
          }
        }
      }
      catch (SocketException ex)
      {
        Debug.Log("Socket exception:" + ex);
      }
    }

    static byte[] header;

    /// <summary>
    /// 判断是否有完整的协议数据可以读取
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private bool ReadMessageBlocking(out byte[] content)
    {
      content = null;

      // create header buffer if not created yet
      if (header == null)
      {
        header = new byte[4];
      }

      // read exactly 4 bytes for header (blocking)
      if (!ReadExactly(_stream, header, 4))
      {
        return false;
      }

      // convert to int
      int size = BytesToIntBigEndian(header);

      if (size > 0 && size <= _maxMessageSize)
      {
        // read exactly 'size' bytes for content (blocking)
        content = new byte[size];
        return ReadExactly(_stream, content, size);
      }

      Debug.Log("ReadMessageBlocking: possible header attack with a header of: " + size + " bytes.");
      return false;
    }


    private bool ReadExactly(NetworkStream stream, byte[] buffer, int amount)
    {
      int bytesRead = 0;
      while (bytesRead < amount)
      {
        // read up to 'remaining' bytes with the 'safe' read extension
        int remaining = amount - bytesRead;
        int result = ReadSafely(stream, buffer, bytesRead, remaining);

        // .Read returns 0 if disconnected
        if (result == 0)
          return false;

        // otherwise add to bytes read
        bytesRead += result;
      }

      return true;
    }

    private int ReadSafely(Stream stream, byte[] buffer, int offset, int size)
    {
      try
      {
        return stream.Read(buffer, offset, size);
      }
      catch (IOException)
      {
        return 0;
      }
    }

    private static int BytesToIntBigEndian(byte[] bytes)
    {
      return (bytes[0] << 24) |
             (bytes[1] << 16) |
             (bytes[2] << 8) |
             bytes[3];
    }


    //------------------------------------------

    private void SendLoop()
    {
      try
      {
        while (true)
        {
          while (_sendQueue.TryDequeue(out byte[] messages))
          {
            _stream.Write(messages, 0, messages.Length);
          }

          Thread.Sleep(1);
        }
      }
      catch (Exception ex)
      {
      }
    }

    public void Send(object packet)
    {
      ByteBuffer byteBuffer = null;
      try
      {
        byteBuffer = ByteBuffer.ValueOf();

        byteBuffer.WriteRawInt(PROTOCOL_HEAD_LENGTH);

        ProtocolManager.Write(byteBuffer, packet);

        // 包的附加包为空
        byteBuffer.WriteBool(false);

        // 包的长度
        int length = byteBuffer.WriteOffset();

        int packetLength = length - PROTOCOL_HEAD_LENGTH;

        byteBuffer.SetWriteOffset(0);
        byteBuffer.WriteRawInt(packetLength);
        byteBuffer.SetWriteOffset(length);

        // var sendSuccess = Send(byteBuffer.ToBytes());
        _sendQueue.Enqueue(byteBuffer.ToBytes());
      }
      catch (Exception ex)
      {
        Debug.Log(ex);
      }
    }

    public void HandleMsg()
    {
      if (_connected2Server)
      {
        while (_receiveQueue.TryDequeue(out Message msg))
        {
          _netPlayer.DispersionMsg(msg);
        }
      }
    }
  }

  public struct Message
  {
    public MessageType messageType;
    public object packet;

    public Message(MessageType messageType, object packet)
    {
      this.messageType = messageType;
      this.packet = packet;
    }
  }

  public enum MessageType
  {
    Connected,
    Data,
    Disconnected
  }
}