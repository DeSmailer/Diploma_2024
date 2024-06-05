using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecisionMaking
{
    public class RivalsGameManager : MonoBehaviour
    {
        List<NPC> NPSs;
        Farm[] farms;
        RivalsWarehouse warehouse;
        RivalsWarehouseView rivalsWarehouseView;

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
    }
}
