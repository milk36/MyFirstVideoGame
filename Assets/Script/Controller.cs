using UnityEngine;

public class Controller : MonoBehaviour
{
  public void LeftMoveDown()
  {
    // Debug.Log("left move!");
    FindObjectOfType<PlayerMovement>().LeftMove();
  }

  public void LeftMoveUp()
  {
    FindObjectOfType<PlayerMovement>().LeftOrRightMoveUp();
  }

  public void RightMoveDown()
  {
    // Debug.Log("right move!");
    FindObjectOfType<PlayerMovement>().RightMove();
  }

  public void RightMoveUp()
  {
    FindObjectOfType<PlayerMovement>().LeftOrRightMoveUp();
  }
}