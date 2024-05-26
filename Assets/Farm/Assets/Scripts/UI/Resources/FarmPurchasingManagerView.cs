using System.Collections.Generic;
using UnityEngine;

namespace FarmRunner {
  public class FarmPurchasingManagerView : MonoBehaviour {
    [SerializeField] FarmPurchasingManager purchasingManager;
    [SerializeField] AmountOfResourcesInPurchasingManagerView amountOfResourcesViewsPrefab;

    [SerializeField] Transform container;

    private List<AmountOfResourcesInPurchasingManagerView> amountOfResourcesViews = new List<AmountOfResourcesInPurchasingManagerView>();

    private void Start() {
      purchasingManager.OnPurchasing.AddListener(OnPurchasing);
      OnPurchasing();
    }

    private void OnPurchasing() {
      ClearContainer();
      if (purchasingManager.CurrentBuyer != null) {
        foreach (var resource in purchasingManager.CurrentBuyer.CostData.CostInResources) {
          AmountOfResourcesInPurchasingManagerView view = Instantiate(amountOfResourcesViewsPrefab, container);
          view.Initialize(resource.Resource);
          view.UpdateAmount(resource.Amount);
          amountOfResourcesViews.Add(view);
        }
      }
    }

    void ClearContainer() {
      foreach (var view in amountOfResourcesViews) {
        Destroy(view.gameObject);
      }
      amountOfResourcesViews.Clear();
    }
  }
}
