using System.Collections.Generic;
using FarmRunner.Utils;
using UnityEngine;

namespace FarmRunner {
  public class GiveAwayZone : Zone {

    [SerializeField] Warehouse warehouse;

    protected override void OnTriggerEnter(Collider other) {
      var inventory = other.GetComponent<Inventory>();
      if (inventory != null) {
        var resources = inventory.GiveAllResources();

        warehouse.AddResources(resources);

        //for (int i = 0; i < resources.Count; i++) {
        //  resources[i].AddToWarehouse(warehouse, GlobalConstants.delayBetweenFlyToWarehouse * i);
        //}
      }
    }
  }
}
