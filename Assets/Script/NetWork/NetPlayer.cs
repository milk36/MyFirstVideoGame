using System;
using NetWork;
using UnityEngine;
using UnityEngine.UI;
using zfoocs;
using Message = NetWork.Message;

public class NetPlayer : MonoBehaviour
{
  public Text serverAddress;

  private NetTcpClient _tcpClient;

  void Start()
  {
  }

  void Update()
  {
    _tcpClient?.HandleMsg();
  }

  public void DispersionMsg(Message msg)
  {
    switch (msg.packet)
    {
      case TestPong obj:
        Debug.Log("接收消息: TestPong time:" + obj);
        break;
    }
  }

  public void Connected2Server()
  {
    Debug.Log("链接服务器...");
    string[] arrays = serverAddress.text.Split(":");
    var serverIP = arrays[0];
    var serverPort = arrays[1];
    _tcpClient = new NetTcpClient(this,serverIP, int.Parse(serverPort));
    _tcpClient.ConnectToServer();
  }

  public void SendPing()
  {
    Debug.Log("send ping msg ...");
    _tcpClient.Send(new TestPing());
  }
}