using UnityEngine;

public class EndTrigger : MonoBehaviour
{
  public GameManager gameManager;
  
  void OnTriggerEnter()
  {
    //终点事件触发器
    gameManager.CompleteLevel();
  }
}
