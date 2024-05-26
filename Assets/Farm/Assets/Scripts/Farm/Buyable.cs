using UnityEngine;

namespace FarmRunner {
  public class Buyable : MonoBehaviour, IBuyable {

    private void Start() {
      gameObject.SetActive(false);
    }

    public void Buy() {
      gameObject.SetActive(true);
      Debug.Log("Bought " + gameObject.name);
    }
  }
}
