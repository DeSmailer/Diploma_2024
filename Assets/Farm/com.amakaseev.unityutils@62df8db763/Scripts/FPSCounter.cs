using UnityEngine;

namespace UnityUtils {

  public class FpsCounter: MonoBehaviour {
    float fps;

    int frames;
    float time;

    void Update() {
      frames++;
      time += Time.unscaledDeltaTime;
      if (time >= 1) {
        fps = frames;
        frames = 0;
        time = 0;
      }
    }

    private void OnGUI() {
      var screenScale = Screen.width / 480.0f;

      Rect location = new Rect(Screen.safeArea.x + 5, Screen.safeArea.y + 5, 85 * screenScale, 25 * screenScale);
      string text = $"FPS: {fps}";
      Texture black = Texture2D.linearGrayTexture;

      GUI.depth = 2;
      GUI.DrawTexture(location, black, ScaleMode.StretchToFill);
      GUI.color = Color.black;
      GUI.skin.label.fontSize = Mathf.RoundToInt(18 * screenScale);
      GUI.Label(location, text);
    }
  }

}
