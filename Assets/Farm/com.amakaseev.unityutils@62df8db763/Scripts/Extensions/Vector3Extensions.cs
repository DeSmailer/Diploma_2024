using UnityEngine;

namespace UnityUtils {

  public static class Vector3Extensions {
    static public Vector2 WorldToCanvas(Vector3 worldPosition, RectTransform canvasRect, Camera camera) {
      var viewport_position = camera.WorldToViewportPoint(worldPosition);

      return new Vector2(
        (viewport_position.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
        (viewport_position.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)
      );
    }
  }
  
}
