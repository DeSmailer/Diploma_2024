using UnityEngine;

namespace UnityUtils {

  [RequireComponent(typeof(RectTransform))]
  public class UIFollow: MonoBehaviour {
    [Header("Follow params")]
    [SerializeField] Transform objectToFollow;
    [SerializeField] Vector3 offset;

    [Header("Optional")]
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform canvasRect;

    RectTransform rt;

    void Start() {
      if (!mainCamera) {
        mainCamera = Camera.main;
      }
      if (!canvasRect) {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
      }

      rt = GetComponent<RectTransform>();
    }

    void LateUpdate() {
      rt.anchoredPosition = Vector3Extensions.WorldToCanvas(objectToFollow.position + offset, canvasRect, mainCamera);
    }

    public void SetFollow(Transform transform) {
      objectToFollow = transform;
    }
    
  }
  
}
