using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public Rigidbody rb;

  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("Start Game!");
    // rb.useGravity = false;
  }

  // 固定帧,用于执行物理碰撞等逻辑 
  void FixedUpdate()
  {
    rb.AddForce(0, 0, 2000 * Time.deltaTime);//deltaTime 平均每帧的速度
  }
}