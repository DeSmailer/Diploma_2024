using System;
using System.Collections.Generic;
using FarmRunner.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace FarmRunner {
  public class Warehouse : ResourcesStorage {

    [SerializeField] LocatorOfCollectedResources locatorOfCollectedResources;

    [SerializeField] List<CollectedResource> flyingResources = new List<CollectedResource>();

    Dictionary<ResourceData, int> resources = new Dictionary<ResourceData, int>();

    public override LocatorOfCollectedResources LocatorOfCollectedResources => locatorOfCollectedResources;
    public Dictionary<ResourceData, int> Resources => resources;

    public override int ResourcesCount {
      get {
        int count = 0;
        foreach (var item in Resources) {
          count += item.Value;
        }
        return count;
      }
    }

    public UnityEvent<ResourceData> OnResourceAdded;
    public UnityEvent<ResourceData> OnResourceRemoved;

    public UnityEvent OnResourcesAdded;
    public UnityEvent OnResourcesRemoved;

    public UnityEvent OnResourcesAmountChanged;

    public void AddResources(List<CollectedResource> resources) {

      float delay = 0;

      foreach (var item in resources) {
        flyingResources.Add(item);
        delay += GlobalConstants.delayBetweenFlyToWarehouse;
        item.AddToWarehouse(this, delay);
        item.OnWarehouseMovementEnded.AddListener(OnWarehouseMovementEnded);
      }
    }

    public void AddResource(CollectedResource resource) {
      AddResource(resource.ResourceData);
    }

    public void AddResource(ResourceData resource) {
      if (Resources.ContainsKey(resource)) {
        Resources[resource]++;
      } else {
        Resources.Add(resource, 1);
      }
      OnResourceAdded.Invoke(resource);
      OnResourcesAdded.Invoke();
      OnResourcesAmountChanged.Invoke();
    }

    private void OnWarehouseMovementEnded(CollectedResource resource) {
      AddResource(resource);
      flyingResources.Remove(resource);
      resource.OnWarehouseMovementEnded.RemoveListener(OnWarehouseMovementEnded);
    }

    public bool HasResources(ResourceData resourceData, int count = 1) {
      if (Resources.ContainsKey(resourceData)) {
        return Resources[resourceData] >= count;
      }
      return false;
    }

    public bool RemoveResources(ResourceData resource, int count) {
      if (HasResources(resource, count)) {
        Resources[resource] -= count;

        OnResourceRemoved.Invoke(resource);
        OnResourcesRemoved.Invoke();
        OnResourcesAmountChanged.Invoke();

        return true;
      }
      return false;
    }

    internal int GetResourceCount(ResourceData resource) {
      if (Resources.ContainsKey(resource)) {
        return Resources[resource];
      }
      return 0;
    }
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
