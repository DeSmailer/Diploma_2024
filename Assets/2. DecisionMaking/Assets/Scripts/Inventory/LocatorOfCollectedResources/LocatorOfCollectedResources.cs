using UnityEngine;

namespace FarmRunner {

  public abstract class LocatorOfCollectedResources : MonoBehaviour {

    [SerializeField] protected Transform inventoryPosition;

    public Transform InventoryPosition => inventoryPosition;

    public abstract Vector3 GetPosition(int index);

    protected Vector3 LocalToGlobal(Vector3 localPosition) {
      return inventoryPosition.TransformPoint(localPosition);
    }
  }
}
