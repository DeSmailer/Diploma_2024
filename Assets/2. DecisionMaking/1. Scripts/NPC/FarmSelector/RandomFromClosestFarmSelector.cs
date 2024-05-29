using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecisionMaking {
  public class RandomFromClosestFarmSelector : IFarmSelector {
    Farm[] farms;
    Transform player;
    int numberOfNearest;

    public RandomFromClosestFarmSelector(Farm[] farms, Transform player, int numberOfNearest) {
      this.farms = farms;
      this.player = player;
      this.numberOfNearest = numberOfNearest;
    }

    public Farm SelectFarm() {
      var selectedFarms = new List<Farm>(farms);
      selectedFarms.RemoveAll(farm => !farm.CanHarvest);

      selectedFarms.Sort((a, b) =>
            Vector3.Distance(a.transform.position, player.position)
                  .CompareTo(Vector3.Distance(b.transform.position, player.position)));

      selectedFarms = selectedFarms.GetRange(0, Mathf.Min(selectedFarms.Count, numberOfNearest));

      return selectedFarms[Random.Range(0, selectedFarms.Count)];
    }
  }

  public class RequiredResourceFarmSelector : IFarmSelector {
    Farm[] farms;
    Transform player;
    RivalsWarehouse rivalsWarehouse;
    ICharacter character;

    public RequiredResourceFarmSelector(Farm[] farms, ICharacter character, Transform player, RivalsWarehouse rivalsWarehouse) {
      this.farms = farms;
      this.player = player;
      this.rivalsWarehouse = rivalsWarehouse;
      this.character = character;
    }

    List<Farm> FarmsWithRequiredResource(ResourceData resourceData) {
      List<Farm> selectedFarms = new List<Farm>();
      foreach (var farm in farms) {
        if (farm.CanHarvest && farm.ResourceData == resourceData) {
          selectedFarms.Add(farm);
        }
      }
      return selectedFarms;
    }

    public Farm SelectFarm() {

      Dictionary<ResourceData, int> leftCollectResources = rivalsWarehouse.LeftCollectResources(character);

      // Сортируем словарь по значению от большего к меньшему
      leftCollectResources = leftCollectResources.OrderByDescending(kv => kv.Value)
                                                 .ToDictionary(kv => kv.Key, kv => kv.Value);

      foreach (var item in leftCollectResources) {
        Debug.Log(character.Transform.gameObject.name + " " + item.Key.ToString() + " " + item.Value.ToString());
      }

      foreach (var item in leftCollectResources) {
        Debug.Log("Required resource: " + item.Key.ToString());
        var farmsWithRequiredResource = FarmsWithRequiredResource(item.Key);
        if (farmsWithRequiredResource.Count > 0) {
          Debug.Log("if");
          farmsWithRequiredResource.Sort((a, b) =>
            Vector3.Distance(a.transform.position, player.position)
                  .CompareTo(Vector3.Distance(b.transform.position, player.position)));
          Debug.Log("Selected farm with required resource: " + farmsWithRequiredResource[0].name);
          return farmsWithRequiredResource[0];
        }
      }

      return farms[Random.Range(0, farms.Length)];
    }
  }
}