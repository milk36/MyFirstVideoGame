using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerP : MonoBehaviour
{
  public static ServerP Instance;

  private float timer;
  private int currentTick;
  private float minTimeBetweenTicks;
  private const float SERVER_TICK_RATE = 30f;
  private const int BUFFER_SIZE = 1024;

  private StatePayload[] _stateBuffer;
  private Queue<InputPayload> _inputQueue;
  
  private void Awake()
  {
    Instance = this;
  }

  // Start is called before the first frame update
  void Start()
  {
    //初始化操作
    minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

    _stateBuffer = new StatePayload[BUFFER_SIZE];
    _inputQueue = new Queue<InputPayload>();
  }

  // Update is called once per frame
  void Update()
  {
    timer += Time.deltaTime;

    while (timer>=minTimeBetweenTicks)
    {
      timer -= minTimeBetweenTicks;
      HandleTick();
      currentTick++;
    }
  }


  /// <summary>
  /// 模拟客户端请求的输入数据
  /// </summary>
  /// <param name="inputPayload"></param>
  public void OnClientInput(InputPayload inputPayload)
  {
    _inputQueue.Enqueue(inputPayload);
  }

  /// <summary>
  /// 发送状态数据到客户端
  /// </summary>
  /// <param name="statePayload"></param>
  /// <returns></returns>
  IEnumerator SendToClient(StatePayload statePayload)
  {
    yield return new WaitForSeconds(0.02f);
    
    ClientP.Instance.OnServerMovementState(statePayload);
  }
  
  private void HandleTick()
  {
    int bufferIndex = -1;
    //判断是否有客户端输入数据
    while (_inputQueue.Count>0)
    {
      InputPayload inputPayload = _inputQueue.Dequeue();

      //获取对应的缓冲索引
      bufferIndex = inputPayload.tick % BUFFER_SIZE;

      //移动后的状态数据
      StatePayload statePayload = ProcessMovement(inputPayload);
      //添加到缓冲中
      _stateBuffer[bufferIndex] = statePayload;
    }

    if (bufferIndex != -1)
    {
      //同步给客户端
      StartCoroutine(SendToClient(_stateBuffer[bufferIndex]));
    }
  }

  /// <summary>
  /// 服务端移动逻辑
  /// </summary>
  /// <param name="inputPayload"></param>
  /// <returns></returns>
  private StatePayload ProcessMovement(InputPayload inputPayload)
  {
    transform.position += inputPayload.inputVector * 5f * minTimeBetweenTicks;
    return new StatePayload()
    {
      tick = inputPayload.tick,
      position = transform.position,
    };
  }
}
