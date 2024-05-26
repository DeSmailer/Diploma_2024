using UnityEngine;

namespace UnityUtils {
  
  [ExecuteInEditMode][RequireComponent(typeof(Camera))]
  public class FixedCameraResolution: MonoBehaviour {
    [SerializeField] float horizontalFOV = 120;
    [SerializeField] float defaultAspect = 3;

    Camera cameraTarget;

    void Start() {
      UpdateResolution();
    }

    void Update() {
      UpdateResolution();
    }

    float CalcVertivalFOV(float hFOVInDeg, float aspectRatio) {
        float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;  
        float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
        float vFOV = vFOVInRads * Mathf.Rad2Deg;
        return vFOV;
    }

    public void UpdateResolution() {
      if (!cameraTarget) {
        cameraTarget = GetComponent<Camera>();
      }
      cameraTarget.fieldOfView = CalcVertivalFOV(horizontalFOV, cameraTarget.aspect);
      cameraTarget.orthographicSize = defaultAspect / cameraTarget.aspect;
    }
  }

}
