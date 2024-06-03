using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
  public PlayerMovement movement;

  void OnCollisionEnter(Collision collisionInfo)
  {
    //碰撞体名称
    // var colliderName = collisionInfo.collider.name;
    //判断碰撞体 tag
    if (collisionInfo.collider.tag == "Obstacle")
    {
      // Debug.Log("hit something!");
      //发生碰撞以后禁用移动组件
      movement.enabled = false;
      //执行游戏结束逻辑
      FindObjectOfType<GameManager>().EndGame();
    }
  }
}