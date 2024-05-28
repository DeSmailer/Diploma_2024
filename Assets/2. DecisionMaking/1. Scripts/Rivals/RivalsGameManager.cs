using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecisionMaking
{
    public class RivalsGameManager : MonoBehaviour
    {
        [SerializeField] List<NPC> NPSs;
        [SerializeField] Farm[] farms;
        [SerializeField] RivalsWarehouse warehouse;
        [SerializeField] RivalsWarehouseView rivalsWarehouseView;

        [SerializeField] CostInResourcesData necessaryResourcesForVictory;

        void Start()
        {
            warehouse = FindObjectOfType<RivalsWarehouse>();
            rivalsWarehouseView = FindObjectOfType<RivalsWarehouseView>();

            NPSs = FindObjectsByType<NPC>(FindObjectsSortMode.None).ToList();
            farms = FindObjectsByType<Farm>(FindObjectsSortMode.None);


            List<ICharacter> characters = new List<ICharacter>(NPSs);

            RivalsNPCInitializer rivalsInitializer = new RivalsNPCInitializer();
            rivalsInitializer.Initialize(NPSs, warehouse, farms, characters);

            RivalsWarehouseInitializer warehouseInitializer = new RivalsWarehouseInitializer();
            warehouseInitializer.Initialize(warehouse, necessaryResourcesForVictory, characters);

            rivalsWarehouseView.Initialize(warehouse, characters);
        }

        //  void Start()
        //  {
        //      warehouse = FindObjectOfType<RivalsWarehouse>();
        //      rivalsWarehouseView = FindObjectOfType<RivalsWarehouseView>();

        //      NPSs = FindObjectsByType<NPC>(FindObjectsSortMode.None).ToList();
        //      farms = FindObjectsByType<Farm>(FindObjectsSortMode.None);

        //      List<ICharacter> characters = new List<ICharacter>(NPSs) {
        //  player
        //};

        //      RivalsNPCInitializer rivalsInitializer = new RivalsNPCInitializer();
        //      rivalsInitializer.Initialize(NPSs, warehouse, farms, characters);

        //      RivalsWarehouseInitializer warehouseInitializer = new RivalsWarehouseInitializer();
        //      warehouseInitializer.Initialize(warehouse, necessaryResourcesForVictory, characters);

        //      rivalsWarehouseView.Initialize(warehouse, characters);
        //      victoryChecker.Initialize(warehouse);

        //      resultsTable.Initialize(victoryChecker, characters, warehouse);
        //  }
    }
}
