using UnityEngine;

namespace FarmRunner {
  public class StockpiledLocator : LocatorOfCollectedResources {
    [SerializeField] protected int xColumnsNumber = 3;
    [SerializeField] protected int zColumnsNumber = 3;
    [SerializeField] protected float distanceBetweenXColumns = 1f;
    [SerializeField] protected float distanceBetweenZColumns = 1f;
    [SerializeField] protected float distanceBetweenRows = 1f;

    protected float startX;
    protected float xDistance;

    protected float startZ;
    protected float zDistance;

    protected float startY;

    private void Awake() {
      xDistance = (xColumnsNumber - 1) * distanceBetweenXColumns;
      startX = -xDistance / 2;

      zDistance = (zColumnsNumber - 1) * distanceBetweenZColumns;
      startZ = -zDistance / 2;
    }

    public override Vector3 GetPosition(int index) {

      float xResult = startX + (index % xColumnsNumber) * distanceBetweenXColumns;
      float zResult = startZ + ((index / xColumnsNumber) % zColumnsNumber) * distanceBetweenZColumns;

      float yResult = startY + ((index / (xColumnsNumber * zColumnsNumber)) * distanceBetweenRows);

      Vector3 result = new Vector3(xResult, yResult, zResult);
      return LocalToGlobal(result);
    }
  }
}
