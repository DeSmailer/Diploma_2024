using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FarmRunner {
  public class FarmPurchasingManager : MonoBehaviour {

    [SerializeField] List<Buyer> farmBuyers;
    [SerializeField] Warehouse warehouse;
    int currentBuyerIndex = 0;

    bool everythingIsBought = false;

    public Buyer CurrentBuyer {
      get {
        if (currentBuyerIndex >= farmBuyers.Count) {
          return null;
        } else {
          return farmBuyers[currentBuyerIndex];
        }
      }
    }

    public UnityEvent OnPurchasing;
    public UnityEvent OnPurchasingLast;

    private void Start() {
      warehouse.OnResourcesAdded.AddListener(TryToBuy);
    }

    void TryToBuy() {
      if (everythingIsBought) {
        return;
      }

      if (CurrentBuyer == null) {
        everythingIsBought = true;
        AlertUI.Instance.ShowAlert("Все фермы открыты", 2f);
        OnPurchasingLast?.Invoke();
        return;
      } else if (CurrentBuyer.CanBuy(warehouse)) {
        CurrentBuyer.Buy(warehouse);
        AlertUI.Instance.ShowAlert("Открыта новая ферма", 2f);
        currentBuyerIndex++;

        OnPurchasing?.Invoke();

        TryToBuy();
      }
    }
  }
}

