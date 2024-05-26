using System.Collections.Generic;

namespace DecisionMaking {
  public class RivalsNPCInitializer {
    public void Initialize(List<NPC> nPSs, RivalsWarehouse warehouse, Farm[] farms, List<ICharacter> characters) {
      foreach (var npc in nPSs) {
        npc.Initialize(warehouse, farms, characters);
      }
    }
  }
}
