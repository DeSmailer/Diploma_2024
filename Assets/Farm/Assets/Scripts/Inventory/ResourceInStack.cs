using UnityEngine;

namespace FarmRunner {
  public class ResourceInStack {
    public ResourceData resource;
    public GameObject resourceObject;
    public int count = 1;

    public ResourceInStack(ResourceData resource, GameObject resourceObject, int count = 1) {
      this.resource = resource;
      this.resourceObject = resourceObject;
      this.count = count;
    }
  }
}