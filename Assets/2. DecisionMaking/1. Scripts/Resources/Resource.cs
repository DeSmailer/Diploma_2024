using UnityEngine;

namespace DecisionMaking {

  public class Resource : Entity {
    ResourceData resource;

    public override void Initialize(EntityData data) {
      resource = (ResourceData)data;
    }

    public void Collect(out CollectedResource spawnedResource) {

      CollectedResource collectedResource = Instantiate(resource.CollectedPrefab, 
          transform.position, Quaternion.identity)
                .GetComponent<CollectedResource>();
      collectedResource.Initialize(resource);
      spawnedResource = collectedResource;
      Destroy(gameObject);
    }
  }
}