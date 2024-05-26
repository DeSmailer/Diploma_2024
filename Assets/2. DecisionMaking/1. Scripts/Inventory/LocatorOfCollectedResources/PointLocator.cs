using UnityEngine;

namespace DecisionMaking {
  public class PointLocator : LocatorOfCollectedResources {

    public override Vector3 GetPosition(int index) {
      return InventoryPosition.position;
    }
  }
}
