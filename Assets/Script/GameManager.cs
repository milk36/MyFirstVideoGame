using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  bool isGameOver = false;

  public float restartDelay = 1f;

  public GameObject completeLevelUI;

  public void EndGame()
  {
    if (!isGameOver)
    {
      isGameOver = true;
      // Debug.Log("Game Over!");
      // 重新加载场景
      // Restart();
      // 延迟调用 restart 方法 
      Invoke("Restart", restartDelay);
    }
  }

  void Restart()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void CompleteLevel()
  {
    Debug.Log("LEVEL 1!");
    completeLevelUI.SetActive(true);
  }
}