using System;

namespace FarmRunner {
  [Serializable]
  public class Buyer {
    public Buyable buyable;
    public CostInResourcesData CostData;

    public bool CanBuy(Warehouse warehouse) {
      foreach (var item in CostData.CostInResources) {
        if (!warehouse.HasResources(item.Resource, item.Amount)) {
          return false;
        }
      }
      return true;
    }

    public void Buy(Warehouse warehouse) {
      foreach (var item in CostData.CostInResources) {
        warehouse.RemoveResources(item.Resource, item.Amount);
      }
      buyable.Buy();
    }
  }
}

