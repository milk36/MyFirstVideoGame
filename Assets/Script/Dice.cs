using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 记录最高分,并存盘操作
/// </summary>
public class Dice : MonoBehaviour
{
  private const string HIGH_SCORE = "HighScore";

  [SerializeField]
  private int testTmp = 1;
  [Header("分数")]
  public Text score;
  [Space]
  [HideInInspector]
  public Text highScore;

  private void Start()
  {
    // Unity 主线程Id = 1 -> Thread Id:1
    Debug.LogFormat("Thread Id:{0}",Thread.CurrentThread.ManagedThreadId);
    //启动时获取保存的数据
    highScore.text = PlayerPrefs.GetInt(HIGH_SCORE, 0).ToString();
  }

  public void RollDice()
  {
    int number = Random.Range(1, 7);
    score.text = number.ToString();

    #region 测试代码区域宏

    if (number > PlayerPrefs.GetInt(HIGH_SCORE, 0))
    {
      //存储高分数据
      PlayerPrefs.SetInt(HIGH_SCORE, number);
      //显示高分数据
      highScore.text = number.ToString();
    }

    #endregion
  }

  public void Reset()
  {
    PlayerPrefs.DeleteKey(HIGH_SCORE);
    highScore.text = "xx";
  }
}