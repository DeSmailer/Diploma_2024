using TMPro;
using UnityEngine;

namespace FarmRunner {
  public class InventoryFilingInspector : MonoBehaviour {
    [SerializeField] Inventory inventory;

    [SerializeField] TMP_Text text;

    private void Awake() {
      inventory.OnInventoryFull.AddListener(OnInventoryFull);
      inventory.OnInventoryEmpty.AddListener(OnInventoryEmpty);
      inventory.OnResourcesCountChanged.AddListener(Display);

      Display();
    }

    private void OnInventoryFull() {
      AlertUI.Instance.ShowAlert("Inventory is full", 2f);
    }

    private void OnInventoryEmpty() {
      AlertUI.Instance.ShowAlert("Inventory is empty", 2f);
    }

    private void Display() {
      text.text = inventory.ResourcesCount + " / " + inventory.MaxCapacity;
    }
  }
}

//public bool RemoveResources(ResourceData resource, int count = 1) {
//  if (HasResources(resource, count)) {
//    int removedCount = 0;

//    for (int i = flyingResources.Count - 1; i >= 0; i--) {
//      if (flyingResources[i].ResourceData == resource) {
//        flyingResources.RemoveAt(i);
//        removedCount++;
//        if (removedCount >= count) {
//          return true;
//        }
//      }
//    }

//    for (int i = flownResources.Count - 1; i >= 0; i--) {
//      if (flownResources[i].ResourceData == resource) {
//        flownResources.RemoveAt(i);
//        removedCount++;
//        if (removedCount >= count) {
//          return true;
//        }
//      }
//    }
//  }
//  return false;
//}

//public void AddResource(CollectedResource resource) {
//  if (CanGetResources) {
//    flyingResources.Add(resource);
//    float delay = GlobalConstants.delayBetweenCollectToInventory * (flyingResources.Count - 1);
//    resource.AddToInventory(this, delay);
//    resource.OnInventoryMovementEnded.AddListener(OnInventoryMovementEnded);
//  } else {
//    Destroy(resource.gameObject);
//  }
//}


//public bool HasResources(ResourceData resourceData, int count = 1) {
//  int foundCount = 0;
//  foreach (CollectedResource item in Resources) {
//    if (item.ResourceData == resourceData) {
//      foundCount++;
//      if (foundCount >= count) {
//        return true;
//      }
//    }
//  }
//  return false;
//}

//public ResourceData RemoveResource(ResourceData resource) {
//  foreach (CollectedResource item in flyingResources) {
//    if (item.ResourceData == resource) {
//      flyingResources.Remove(item);
//      return item.ResourceData;
//    }
//  }
//  foreach (CollectedResource item in flownResources) {
//    if (item.ResourceData == resource) {
//      flownResources.Remove(item);
//      return item.ResourceData;
//    }
//  }

//  return null;
//}