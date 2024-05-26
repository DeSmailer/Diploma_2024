using UnityEngine;

[RequireComponent (typeof(RectTransform))]
public class SafeArea: MonoBehaviour {
  [SerializeField] bool enable = false;

  void Awake() {
    if (enable) {
      var rt = GetComponent<RectTransform>();
      rt.anchorMin = new Vector2(Screen.safeArea.min.x / Screen.width, Screen.safeArea.min.y / Screen.height);
      rt.anchorMax = new Vector2(Screen.safeArea.max.x / Screen.width, Screen.safeArea.max.y / Screen.height);
    }
  }
}
