using System.Collections.Generic;
using UnityEngine;

namespace FarmRunner {
  public class WarehouseView : MonoBehaviour {
    [SerializeField] Warehouse warehouse;
    [SerializeField] AmountOfResourcesInWarehouseView amountOfResourcesViewsPrefab;

    [SerializeField] Transform container;

    private List<AmountOfResourcesInWarehouseView> amountOfResourcesViews = new List<AmountOfResourcesInWarehouseView>();

    private void Start() {
      warehouse.OnResourceAdded.AddListener(OnResourceAmountChanged);
      warehouse.OnResourceRemoved.AddListener(OnResourceAmountChanged);
    }

    private void OnResourceAmountChanged(ResourceData resource) {
      AmountOfResourcesInWarehouseView view = amountOfResourcesViews.Find(x => x.Resource == resource);
      if (view != null) {
        view.UpdateAmount(warehouse.GetResourceCount(resource));
      } else {
        AmountOfResourcesInWarehouseView newView = Instantiate(amountOfResourcesViewsPrefab, container);
        newView.Initialize(resource);
        amountOfResourcesViews.Add(newView);
      }
    }
  }
}
