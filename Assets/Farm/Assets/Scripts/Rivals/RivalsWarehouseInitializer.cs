using System.Collections.Generic;

namespace FarmRunner {
  public class RivalsWarehouseInitializer {
    public void Initialize(RivalsWarehouse warehouse, CostInResourcesData necessaryResourcesForVictory, List<ICharacter> characters) {
      warehouse.Initialize(necessaryResourcesForVictory, characters);
    }
  }
}
