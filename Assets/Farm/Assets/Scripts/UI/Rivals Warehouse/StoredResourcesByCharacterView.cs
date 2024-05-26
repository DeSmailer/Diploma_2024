using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FarmRunner
{
    public class StoredResourcesByCharacterView : MonoBehaviour
    {
        [SerializeField] Image characterImage;
        [SerializeField] TMP_Text characterName;

        [SerializeField] StoredResourcesView storedResourcesViewPrefab;
        [SerializeField] Transform storedResourcesViewContainer;

        ICharacter character;
        RivalsWarehouse rivalsWarehouse;
        List<StoredResourcesView> storedResourcesViews;

        public void Initialize(ICharacter character, RivalsWarehouse rivalsWarehouse)
        {
            this.character = character;
            this.rivalsWarehouse = rivalsWarehouse;

            characterImage.sprite = character.CharacterInfo.Sprite;
            characterName.text = character.CharacterInfo.Name;

            CreateStoredResourcesViews();

            Display();

            rivalsWarehouse.OnResourcesAdded.AddListener(Display);
        }

        private void CreateStoredResourcesViews()
        {
            storedResourcesViews = new List<StoredResourcesView>();
            foreach(var item in rivalsWarehouse.CharacterResources)
            {
                if(item.Key == character)
                {
                    var resources = item.Value;
                    foreach(var resource in resources)
                    {
                        StoredResourcesView storedResourcesView = Instantiate(storedResourcesViewPrefab, storedResourcesViewContainer);
                        storedResourcesView.Initialize(resource.Key);
                        storedResourcesViews.Add(storedResourcesView);
                    }
                }
            }
        }

        public void Display()
        {
            foreach(var storedResourcesView in storedResourcesViews)
            {
                storedResourcesView.Display(rivalsWarehouse.LeftCollectResource(storedResourcesView.ResourceData, character));
            }
        }
    }
}