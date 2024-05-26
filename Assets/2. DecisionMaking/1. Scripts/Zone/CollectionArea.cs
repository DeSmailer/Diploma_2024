using UnityEngine;

namespace FarmRunner {
  public class CollectionArea : Zone {

    [SerializeField] Farm farm;
    Inventory inventory;

    protected override void OnTriggerEnter(Collider other) {
      inventory = other.GetComponent<Inventory>();
      TryCollectResources();
    }

    private void OnTriggerExit(Collider other) {
      var inv = other.GetComponent<Inventory>();
      if (inventory == inv) {
        inventory = null;
      }
    }

    void TryCollectResources() {
      if (inventory != null) {
        farm.TryCollectResources(inventory);
      }
    }
  }
}

//protected override void OnTriggerEnter(Collider other) {
//  var inv = other.GetComponent<Inventory>();
//  if (inv != null) {
//    inventory = inv;
//  }
//  TryCollectResources();
//}

//private void OnTriggerStay(Collider other) {
//  TryCollectResources();
//}

//private void OnTriggerExit(Collider other) {
//  var inv = other.GetComponent<Inventory>();
//  if (inventory == inv) {
//    inventory = null;
//  }
//}
