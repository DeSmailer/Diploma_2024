using UnityEngine;

namespace DecisionMaking {
  public class RivalsGiveAwayZone : Zone {

    [SerializeField] RivalsWarehouse warehouse;

    protected override void OnTriggerEnter(Collider other) {
      var character = other.GetComponent<ICharacter>();
      if (character != null) {
        var resources = character.Inventory.GiveAllResources();
        warehouse.AddResources(resources, character);
      }
    }
  }
}
