using UnityEngine;

namespace FarmRunner {
  public class PointLocator : LocatorOfCollectedResources {

    public override Vector3 GetPosition(int index) {
      return InventoryPosition.position;
    }
  }
}
