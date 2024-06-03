using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
  public Transform player;

  public Text scoreText;
  
  // Update is called once per frame
  void Update()
  {
    // 将玩家移动距离当作游戏得分显示
    scoreText.text = player.position.z.ToString("0");
  }
}