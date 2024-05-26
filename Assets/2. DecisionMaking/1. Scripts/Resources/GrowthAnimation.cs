using UnityEngine;

namespace DecisionMaking {
  public class GrowthAnimation : MonoBehaviour {

    [SerializeField] LeanTweenType ease;

    [SerializeField] float duration;

    private void Awake() {
      Grow(duration);
    }

    public void Grow(float duration = 1) {

      Vector3 endScale = transform.localScale;
      transform.localScale = Vector3.zero;
      LeanTween.scale(gameObject, endScale, duration).setEase(ease);
    }
  }
}

//public void Collect(Inventory inventory) {

//  CollectedResource collectedResource = Instantiate(resource.CollectedPrefab, transform.position, Quaternion.identity).GetComponent<CollectedResource>();
//  collectedResource.Initialize(resource);
//  inventory.AddResource(collectedResource);
//  Destroy(gameObject);
//}

