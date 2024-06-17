using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class ThreadQueuer : MonoBehaviour
{
  //主线程任务队列
  private ConcurrentQueue<Action> _actions;

  void Start()
  {
    Debug.Log("启动...");
    _actions = new ConcurrentQueue<Action>();
    
    StartThreadFunction(() =>
    {
      slowTask(Vector3.up * 2, new float[2], new Color[10]);
    });
    
    Debug.Log("启动完成!");
  }

  void Update()
  {
    while (_actions.TryDequeue(out Action action))
    {
      action();
    }
  }

  /// <summary>
  /// 开启一个线程执行逻辑
  /// </summary>
  /// <param name="function"></param>
  public void StartThreadFunction(Action function)
  {
    Thread t = new Thread(new ThreadStart(function));
    t.Start();
  }

  /// <summary>
  /// 添加任务到主线程
  /// </summary>
  /// <param name="task"></param>
  public void AddTask4MainQueue(Action task)
  {
    _actions.Enqueue(task);
  }

  void slowTask(Vector3 foo, float[] bar, Color[] pixels)
  {
    Thread.Sleep(2000); //模拟耗时操作

    AddTask4MainQueue(() =>
    {
      Debug.Log("子线程完成操作, 将结果通知主线程执行");
      transform.position += foo;
    });
    
    Thread.Sleep(2000); //模拟耗时操作

    AddTask4MainQueue(() =>
    {
      Debug.Log("子线程完成操作, 将结果通知主线程执行");
      transform.position += foo;
    });
  }
}