using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public Rigidbody rb;

  public float forwarForce = 2000f;
  public float sidewaysForce = 500f;

  // Start is called before the first frame update
  void Start()
  {
    Debug.Log("Start Game!");
    // rb.useGravity = false;
  }

  // 固定帧,用于执行物理碰撞等逻辑 
  void FixedUpdate()
  {
    rb.AddForce(0, 0, forwarForce * Time.deltaTime); //deltaTime 平均每帧的速度

    if (Input.GetKey("d"))
    {
      rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
    }

    if (Input.GetKey("a"))
    {
      rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
    }

    if (rb.position.y < -1f)
    {
      FindObjectOfType<GameManager>().EndGame();
    }
  }
}