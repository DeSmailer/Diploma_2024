using UnityEngine;
using UnityEngine.SceneManagement;

namespace DecisionMaking {
  public class SceneRestarter : MonoBehaviour {
    public void RestartScene() {
      int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(currentSceneIndex);
    }
  }
}
