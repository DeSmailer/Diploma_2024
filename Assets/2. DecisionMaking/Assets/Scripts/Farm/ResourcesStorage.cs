using UnityEngine;

namespace FarmRunner {
  public abstract class ResourcesStorage : MonoBehaviour, IResourceStorage {
    public abstract LocatorOfCollectedResources LocatorOfCollectedResources { get; }
    public abstract int ResourcesCount { get; }
  }
}


//public ResourceData RemoveResource(ResourceData resource) {
//  if (resourceCounts.ContainsKey(resource) && resourceCounts[resource] > 0) {
//    resourceCounts[resource]--;

//    OnResourceRemoved.Invoke(resource);
//    OnResourcesRemoved.Invoke();
//    OnResourcesAmountChanged.Invoke();

//    return resource;
//  }
//  return null;
//}
