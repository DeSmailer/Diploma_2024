using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionMaking
{
    public class RivalsWarehouseView : MonoBehaviour
    {
        [SerializeField] StoredResourcesByCharacterView storedResourcesByCharacterViewPrefab;
        [SerializeField] Transform storedResourcesByCharacterViewContainer;

        //RivalsWarehouse warehouse;
        //List<ICharacter> characters;

        public void Initialize(RivalsWarehouse warehouse, List<ICharacter> characters)
        {
            //this.warehouse = warehouse;
            //this.characters = characters;

            foreach(ICharacter character in characters)
            {
                var storedResourcesByCharacterView = Instantiate(storedResourcesByCharacterViewPrefab, storedResourcesByCharacterViewContainer);
                storedResourcesByCharacterView.Initialize(character, warehouse);
            }
        }
    }
}
