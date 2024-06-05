using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// 记录最高分,并存盘操作
/// </summary>
public class Dice : MonoBehaviour
{
  private const string HIGH_SCORE = "HighScore";
  public Text score;
  public Text highScore;

  private void Start()
  {
    //启动时获取保存的数据
    highScore.text = PlayerPrefs.GetInt(HIGH_SCORE, 0).ToString();
  }

  public void RollDice()
  {
    int number = Random.Range(1, 7);
    score.text = number.ToString();

    if (number > PlayerPrefs.GetInt(HIGH_SCORE, 0))
    {
      //存储高分数据
      PlayerPrefs.SetInt(HIGH_SCORE,number);
      //显示高分数据
      highScore.text = number.ToString();
    }
  }

  public void Reset()
  {
    PlayerPrefs.DeleteKey(HIGH_SCORE);
    highScore.text = "xx";
  }
}
