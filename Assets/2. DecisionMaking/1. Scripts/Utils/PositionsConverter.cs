using UnityEngine;

namespace DecisionMaking.Utils {
  public static class PositionsConverter {
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position) {
      position.z = camera.nearClipPlane;
      return camera.ScreenToWorldPoint(position);
    }
  }
}
