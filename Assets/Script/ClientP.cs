using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家输入数据
/// </summary>
public struct InputPayload
{
  public int tick;
  public Vector3 inputVector;
}

/// <summary>
/// 状态信息
/// </summary>
public struct StatePayload
{
  public int tick;

  //位置信息
  public Vector3 position;
}

public class ClientP : MonoBehaviour
{
  public static ClientP Instance;

  //每帧最小间隔时间
  private float minTimeBetweenTicks;
  private const float SERVER_TICK_RATE = 30f;
  private const int BUFFER_SIZE = 1024;

  //循环buff
  private StatePayload[] _stateBuffer;

  private InputPayload[] _inputBuffer;

  //服务端最新状态帧数据 权威状态
  private StatePayload latestServerState;

  //客户端最后处理的帧数据(和解帧)
  private StatePayload lastProcessedState;
  private float horizontalInput;

  private float verticalInput;

  //累计间隔时间
  private float timer;
  private int currentTick;

  private void Awake()
  {
    //初始化单例
    Instance = this;
  }

  // Start is called before the first frame update
  void Start()
  {
    //一秒30帧间隔的最小时间
    minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

    //初始化两个buff
    _stateBuffer = new StatePayload[BUFFER_SIZE];
    _inputBuffer = new InputPayload[BUFFER_SIZE];
  }

  // Update is called once per frame
  void Update()
  {
    //获取移动输入
    horizontalInput = Input.GetAxis("Horizontal");
    verticalInput = Input.GetAxis("Vertical");

    //累计帧间隔
    timer += Time.deltaTime;

    while (timer >= minTimeBetweenTicks)
    {
      //重置间隔时间
      timer -= minTimeBetweenTicks;
      HandleTick();
      currentTick++;
    }
  }

  /// <summary>
  /// 服务端同步移动状态数据
  /// </summary>
  /// <param name="serverState"></param>
  public void OnServerMovementState(StatePayload serverState)
  {
    latestServerState = serverState;
  }

  /// <summary>
  /// 协程的方式像服务端发送操作指令
  /// </summary>
  /// <param name="inputPayload"></param>
  /// <returns></returns>
  IEnumerator SendToServer(InputPayload inputPayload)
  {
    //20毫秒后再发送数据 模拟延迟
    yield return new WaitForSeconds(0.02f);

    ServerP.Instance.OnClientInput(inputPayload);
  }

  void HandleTick()
  {
    //判断是否需要执行回滚操作
    if (!latestServerState.Equals(default(StatePayload)) &&
        (lastProcessedState.Equals(default(StatePayload)) ||
         !latestServerState.Equals(lastProcessedState)))
    {
      //如果最新服务端状态帧数据不为空 && 最后处理状态帧为空
      //或者最新服务端状态帧数据不等于最后处理状态帧
      //执行回滚逻辑
      //TODO 这里每次都会执行
      HandleServerReconciliation();
    }

    int bufferIndex = currentTick % BUFFER_SIZE; //获取缓冲索引

    InputPayload inputPayload = new InputPayload();
    inputPayload.tick = currentTick;
    inputPayload.inputVector = new Vector3(horizontalInput, 0, verticalInput);
    //将输入数据添加到缓冲
    _inputBuffer[bufferIndex] = inputPayload;

    //添加状态帧缓冲
    _stateBuffer[bufferIndex] = ProcessMovement(inputPayload);

    //发送数据到服务端
    StartCoroutine(SendToServer(inputPayload));
  }

  /// <summary>
  /// 处理移动逻辑
  /// </summary>
  /// <param name="inputPayload"></param>
  /// <returns></returns>
  private StatePayload ProcessMovement(InputPayload inputPayload)
  {
    //改变客户端玩家位置
    transform.position += inputPayload.inputVector * 5f * minTimeBetweenTicks;
    
    return new StatePayload()
    {
      tick = inputPayload.tick,
      position = transform.position,
    };
  }

  /// <summary>
  /// 每帧执行的协调逻辑
  /// 默认情况下 服务端的最新状态会落后与客户端的帧数
  /// 也就是说服务端的最新状态是客户端之前某一帧的状态,
  /// 所以需要先获取客户端的 stateBuffer 缓存与其比对
  /// </summary>
  private void HandleServerReconciliation()
  {
    lastProcessedState = latestServerState;
    // Debug.Log("比较服务端最新状态的偏差...");

    int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;
    //获取偏差值
    float positionError = Vector3.Distance(latestServerState.position,
      _stateBuffer[serverStateBufferIndex].position);

    //TODO 只有偏差比较大,才会执行里面的逻辑
    if (positionError > 0.001f)
    {
      Debug.Log("have to reconcile bro");

      //用服务端数据覆盖客户端
      transform.position = latestServerState.position;

      //覆盖客户端缓冲
      _stateBuffer[serverStateBufferIndex] = latestServerState;

      //从服务端最新一帧开始重新执行客户端状态
      int tickToProcess = latestServerState.tick + 1;

      while (tickToProcess < currentTick)
      {
        int bufferIndex = tickToProcess % BUFFER_SIZE;

        //和解后重新移动
        StatePayload statePayload = ProcessMovement(_inputBuffer[bufferIndex]);
        
        //更新缓冲区
        _stateBuffer[bufferIndex] = statePayload;

        tickToProcess++;
      }
    }
  }
}