using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
            List<ICharacter> characters = new List<ICharacter>(NPSs);

            RivalsNPCInitializer rivalsInitializer = new RivalsNPCInitializer();
            rivalsInitializer.Initialize(NPSs, warehouse, farms, characters);

            RivalsWarehouseInitializer warehouseInitializer = new RivalsWarehouseInitializer();
            warehouseInitializer.Initialize(warehouse, necessaryResourcesForVictory, characters);

            rivalsWarehouseView.Initialize(warehouse, characters);
        }
    }
}
