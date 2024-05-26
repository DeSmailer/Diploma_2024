using System.Collections.Generic;

namespace DecisionMaking {
  public class RivalsWarehouseInitializer {
    public void Initialize(RivalsWarehouse warehouse, CostInResourcesData necessaryResourcesForVictory, List<ICharacter> characters) {
      warehouse.Initialize(necessaryResourcesForVictory, characters);
    }
  }
}
